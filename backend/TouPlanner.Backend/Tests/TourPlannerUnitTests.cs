using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using Domain.Helpers;
using Domain.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Model;
using Model.Entities;
using Moq;
using View.Auth;
using View.Controllers.Auth;
using View.Controllers.Entities;
using View.DTOs;
using View.Requests;
using View.Responses;
using View.Services;
using Xunit;

namespace Tests;

public class TourPlannerUnitTests {
    private static readonly CancellationToken Ct = CancellationToken.None;

    private static void SetUser(ControllerBase controller, Guid userId) {
        var identity = new ClaimsIdentity(
            [new Claim(ClaimTypes.NameIdentifier, userId.ToString())], "test");
        controller.ControllerContext = new ControllerContext {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
        };
    }

    private static void SetRawUserClaim(ControllerBase controller, string? rawIdClaim) {
        Claim[] claims = rawIdClaim is null
            ? []
            : [new Claim(ClaimTypes.NameIdentifier, rawIdClaim)];
        controller.ControllerContext = new ControllerContext {
            HttpContext = new DefaultHttpContext {
                User = new ClaimsPrincipal(new ClaimsIdentity(claims, "test"))
            }
        };
    }

    #region TokenService

    private static TokenService CreateTokenService() {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> {
                ["Jwt:Key"] = "unit-test-signing-key-that-is-long-enough-for-hs256!",
                ["Jwt:Issuer"] = "TourPlanner",
                ["Jwt:Audience"] = "TourPlannerClients",
                ["Jwt:ExpiresMinutes"] = "60"
            })
            .Build();
        return new TokenService(config);
    }

    // The whole ownership model downstream depends on the sub/NameIdentifier claim.
    [Fact]
    public void CreateToken_ValidUser_EmbedsUserIdentityClaims() {
        // Arrange
        var user = new ApplicationUser { Username = "alice", Email = "alice@example.com" };
        var sut = CreateTokenService();

        // Act
        var token = sut.CreateToken(user);
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

        // Assert
        Assert.Equal(user.Id.ToString(), jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value);
        Assert.Equal("alice@example.com", jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email).Value);
        Assert.Contains(jwt.Claims, c => c.Type == JwtRegisteredClaimNames.UniqueName && c.Value == "alice");
    }

    [Fact]
    public void CreateToken_ValidUser_SetsIssuerAudienceExpiryAndHs256() {
        // Arrange
        var sut = CreateTokenService();
        var before = DateTime.UtcNow;

        // Act
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(
            sut.CreateToken(new ApplicationUser { Username = "bob", Email = "bob@example.com" }));

        // Assert
        Assert.Equal("TourPlanner", jwt.Issuer);
        Assert.Contains("TourPlannerClients", jwt.Audiences);
        Assert.Equal("HS256", jwt.Header.Alg);
        Assert.InRange(jwt.ValidTo, before.AddMinutes(59), DateTime.UtcNow.AddMinutes(61));
    }

    #endregion

    #region TsQueryHelper

    [Fact]
    public void BuildPrefixTsQuery_SingleWord_AddsPrefixOperator() {
        Assert.Equal("lake:*", TsQueryHelper.BuildPrefixTsQuery("lake"));
    }

    [Fact]
    public void BuildPrefixTsQuery_MultipleWords_JoinsWithAnd() {
        Assert.Equal("lake:* & tour:*", TsQueryHelper.BuildPrefixTsQuery("lake tour"));
    }

    [Fact]
    public void BuildPrefixTsQuery_OnlySymbols_ReturnsEmptyString() {
        Assert.Equal("", TsQueryHelper.BuildPrefixTsQuery("!!! ---"));
    }

    #endregion

    #region AuthController

    private static (AuthController Controller, Mock<IApplicationUserRepository> Users, Mock<ITokenService> Tokens)
        CreateAuthController() {
        var tokens = new Mock<ITokenService>();
        tokens.Setup(t => t.CreateToken(It.IsAny<ApplicationUser>())).Returns("jwt-token");
        var users = new Mock<IApplicationUserRepository>();
        var controller = new AuthController(tokens.Object, users.Object, NullLogger<AuthController>.Instance);
        return (controller, users, tokens);
    }

    [Fact]
    public async Task Register_UsernameTaken_ReturnsBadRequestAndCreatesNothing() {
        // Arrange
        var (sut, users, _) = CreateAuthController();
        users.Setup(u => u.ExistsAsync(It.IsAny<Expression<Func<ApplicationUser, bool>>>(), Ct))
            .ReturnsAsync(true);

        // Act
        var result = await sut.Register(
            new RegisterRequest { Username = "alice", Email = "a@b.c", Password = "secret1" }, Ct);

        // Assert
        var bad = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Username already exists.", bad.Value);
        users.Verify(u => u.CreateAsync(It.IsAny<ApplicationUser>(), Ct), Times.Never);
    }

    [Fact]
    public async Task Register_EmailTaken_ReturnsBadRequest() {
        // Arrange
        var (sut, users, _) = CreateAuthController();
        users.SetupSequence(u => u.ExistsAsync(It.IsAny<Expression<Func<ApplicationUser, bool>>>(), Ct))
            .ReturnsAsync(false) // username check
            .ReturnsAsync(true); // email check

        // Act
        var result = await sut.Register(
            new RegisterRequest { Username = "alice", Email = "a@b.c", Password = "secret1" }, Ct);

        // Assert
        var bad = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Email already exists.", bad.Value);
        users.Verify(u => u.CreateAsync(It.IsAny<ApplicationUser>(), Ct), Times.Never);
    }

    // Password must be hashed — plaintext must never reach the database.
    [Fact]
    public async Task Register_NewUser_CreatesUserAndReturnsToken() {
        // Arrange
        var (sut, users, _) = CreateAuthController();
        users.Setup(u => u.ExistsAsync(It.IsAny<Expression<Func<ApplicationUser, bool>>>(), Ct))
            .ReturnsAsync(false);

        // Act
        var result = await sut.Register(
            new RegisterRequest { Username = "alice", Email = "a@b.c", Password = "secret1" }, Ct);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<AuthResponse>(ok.Value);
        Assert.Equal("jwt-token", response.Token);
        Assert.Equal("alice", response.Username);
        users.Verify(u => u.CreateAsync(It.Is<ApplicationUser>(x => x.PasswordHash != "secret1"), Ct),
            Times.Once);
    }

    // 401 must not leak whether the account exists.
    [Fact]
    public async Task Login_UnknownUser_ReturnsUnauthorized() {
        // Arrange
        var (sut, users, tokens) = CreateAuthController();
        users.Setup(u => u.FirstOrDefaultAsync(It.IsAny<Expression<Func<ApplicationUser, bool>>>(), Ct))
            .ReturnsAsync((ApplicationUser?)null);

        // Act
        var result = await sut.Login(new LoginRequest { UsernameOrEmail = "ghost", Password = "x" }, Ct);

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(result.Result);
        tokens.Verify(t => t.CreateToken(It.IsAny<ApplicationUser>()), Times.Never);
    }

    [Fact]
    public async Task Login_WrongPassword_ReturnsUnauthorized() {
        // Arrange
        var (sut, users, tokens) = CreateAuthController();
        var user = new ApplicationUser {
            Username = "alice",
            Email = "a@b.c",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("correct-password")
        };
        users.Setup(u => u.FirstOrDefaultAsync(It.IsAny<Expression<Func<ApplicationUser, bool>>>(), Ct))
            .ReturnsAsync(user);

        // Act
        var result = await sut.Login(new LoginRequest { UsernameOrEmail = "alice", Password = "wrong" }, Ct);

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(result.Result);
        tokens.Verify(t => t.CreateToken(It.IsAny<ApplicationUser>()), Times.Never);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsAuthResponse() {
        // Arrange
        var (sut, users, _) = CreateAuthController();
        var user = new ApplicationUser {
            Username = "alice",
            Email = "a@b.c",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("secret1")
        };
        users.Setup(u => u.FirstOrDefaultAsync(It.IsAny<Expression<Func<ApplicationUser, bool>>>(), Ct))
            .ReturnsAsync(user);

        // Act
        var result = await sut.Login(new LoginRequest { UsernameOrEmail = "a@b.c", Password = "secret1" }, Ct);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<AuthResponse>(ok.Value);
        Assert.Equal("jwt-token", response.Token);
        Assert.Equal(user.Id, response.UserId);
        Assert.Equal("alice", response.Username);
    }

    // A valid JWT with a garbage subclaim must not reach the database.
    [Theory]
    [InlineData(null)]
    [InlineData("not-a-guid")]
    public async Task Me_MissingOrMalformedIdClaim_ReturnsUnauthorized(string? rawClaim) {
        var (sut, users, _) = CreateAuthController();
        SetRawUserClaim(sut, rawClaim);

        var result = await sut.Me(Ct);

        Assert.IsType<UnauthorizedResult>(result.Result);
        users.Verify(
            u => u.FirstOrDefaultAsync(It.IsAny<Expression<Func<ApplicationUser, bool>>>(), Ct),
            Times.Never);
    }

    #endregion

    #region TourController

    private static (TourController Controller, Mock<ITourRepository> Repo) CreateTourController() {
        var repo = new Mock<ITourRepository>();
        var controller = new TourController(
            repo.Object, new Mock<IRouteService>().Object, NullLogger<TourController>.Instance);
        return (controller, repo);
    }

    private static CreateTourDto SampleCreateTourDto() => new(
        Name: "Alps Tour",
        Description: "Scenic ride",
        FromLongitude: 16.37, FromLatitude: 48.21,
        ToLongitude: 15.44, ToLatitude: 47.07,
        TransportType: TransportType.Cycling,
        Distance: 0, Duration: 0, Coordinates: "", ChildFriendliness: 0);

    [Fact]
    public async Task CreateAsync_NoUserClaim_ReturnsUnauthorized() {
        var (sut, repo) = CreateTourController();
        SetRawUserClaim(sut, null);

        var result = await sut.CreateAsync(SampleCreateTourDto(), Ct);

        Assert.IsType<UnauthorizedObjectResult>(result.Result);
        repo.Verify(r => r.CreateAsync(It.IsAny<Tour>(), Ct), Times.Never);
    }

    [Fact]
    public async Task ReadAsync_TourOwnedByOtherUser_ReturnsNotFound() {
        var (sut, repo) = CreateTourController();
        SetUser(sut, Guid.NewGuid());
        var foreignTour = new Tour { Name = "Foreign", UserId = Guid.NewGuid() };
        repo.Setup(r => r.ReadAsync(foreignTour.Id, Ct)).ReturnsAsync(foreignTour);

        var result = await sut.ReadAsync(foreignTour.Id, Ct);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task SearchAsync_BlankQuery_ReturnsBadRequest(string query) {
        var (sut, repo) = CreateTourController();
        SetUser(sut, Guid.NewGuid());

        var result = await sut.SearchAsync(query, Ct);

        Assert.IsType<BadRequestObjectResult>(result.Result);
        repo.Verify(r => r.SearchAsync(It.IsAny<string>(), Ct), Times.Never);
    }

    #endregion

    #region TourLogController

    private static (TourLogController Controller, Mock<ITourLogRepository> Logs, Mock<ITourRepository> Tours)
        CreateTourLogController() {
        var logs = new Mock<ITourLogRepository>();
        var tours = new Mock<ITourRepository>();
        var controller =
            new TourLogController(logs.Object, tours.Object, NullLogger<TourLogController>.Instance);
        return (controller, logs, tours);
    }

    private static CreateTourLogDto SampleCreateLogDto(Guid tourId) =>
        new(tourId, DateTime.UtcNow, "Nice trail", Difficulty: 3, TotalDistanceKm: 12.5,
            TotalTimeMinutes: 90, Rating: 4);

    [Fact]
    public async Task CreateAsync_TourDoesNotExist_ReturnsNotFoundAndSkipsCreate() {
        var (sut, logs, tours) = CreateTourLogController();
        var tourId = Guid.NewGuid();
        tours.Setup(t => t.ExistsAsync(tourId, Ct)).ReturnsAsync(false);

        var result = await sut.CreateAsync(SampleCreateLogDto(tourId), Ct);

        Assert.IsType<NotFoundObjectResult>(result.Result);
        logs.Verify(l => l.CreateAsync(It.IsAny<TourLog>(), Ct), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ExistingOwnedLog_DeletesAndReturnsNoContent() {
        var (sut, logs, _) = CreateTourLogController();
        var userId = Guid.NewGuid();
        SetUser(sut, userId);
        var log = new TourLog { UserId = userId, TourId = Guid.NewGuid() };
        logs.Setup(l => l.ReadAsync(log.Id, Ct)).ReturnsAsync(log);

        var result = await sut.DeleteAsync(log.Id, Ct);

        Assert.IsType<NoContentResult>(result);
        logs.Verify(l => l.DeleteAsync(log, Ct), Times.Once);
    }

    // Regression: TourLogController.DeleteAsync previously had no ownership check.
    [Fact]
    public async Task DeleteAsync_LogOwnedByOtherUser_ReturnsNotFoundAndDeletesNothing() {
        var (sut, logs, _) = CreateTourLogController();
        SetUser(sut, Guid.NewGuid());
        var foreignLog = new TourLog { TourId = Guid.NewGuid(), UserId = Guid.NewGuid() };
        logs.Setup(l => l.ReadAsync(foreignLog.Id, Ct)).ReturnsAsync(foreignLog);

        var result = await sut.DeleteAsync(foreignLog.Id, Ct);

        Assert.IsType<NotFoundResult>(result);
        logs.Verify(l => l.DeleteAsync(It.IsAny<TourLog>(), Ct), Times.Never);
    }

    #endregion
}