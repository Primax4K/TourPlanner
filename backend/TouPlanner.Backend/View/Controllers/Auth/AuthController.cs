using View.Auth;
using View.Requests;
using View.Responses;

namespace View.Controllers.Auth;

[ApiController]
[Route("api/auth")]
public class AuthController(
	ITokenService tokenService,
	IApplicationUserRepository userRepository,
	ILogger<AuthController> logger) : ControllerBase {

	[HttpPost("register")]
	public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request, CancellationToken ct) {
		var usernameExists = await userRepository.ExistsAsync(x => x.Username == request.Username, ct);

		if (usernameExists) {
			logger.LogWarning("Registration failed — username already taken: {Username}", request.Username);
			return BadRequest("Username already exists.");
		}

		var emailExists = await userRepository.ExistsAsync(x => x.Email == request.Email, ct);

		if (emailExists) {
			logger.LogWarning("Registration failed — email already registered: {Email}", request.Email);
			return BadRequest("Email already exists.");
		}

		var user = new ApplicationUser {
			Username = request.Username,
			Email = request.Email,
			PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
		};

		await userRepository.CreateAsync(user, ct);
		logger.LogInformation("New user registered: {UserId} ({Username})", user.Id, user.Username);

		var token = tokenService.CreateToken(user);

		return Ok(new AuthResponse {
			Token = token,
			UserId = user.Id,
			Username = user.Username,
			Email = user.Email
		});
	}

	[HttpPost("login")]
	public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken ct) {
		var user = await userRepository.FirstOrDefaultAsync(x =>
			x.Username == request.UsernameOrEmail || x.Email == request.UsernameOrEmail, ct);

		if (user is null) {
			logger.LogWarning("Login failed — user not found: {UsernameOrEmail}", request.UsernameOrEmail);
			return Unauthorized("Invalid credentials.");
		}

		var passwordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

		if (!passwordValid) {
			logger.LogWarning("Login failed — wrong password for user {UserId} ({Username})", user.Id, user.Username);
			return Unauthorized("Invalid credentials.");
		}

		logger.LogInformation("User logged in: {UserId} ({Username})", user.Id, user.Username);
		var token = tokenService.CreateToken(user);

		return Ok(new AuthResponse {
			Token = token,
			UserId = user.Id,
			Username = user.Username,
			Email = user.Email
		});
	}

	[Authorize]
	[HttpGet("me")]
	public async Task<ActionResult<MeResponse>> Me(CancellationToken ct) {
		var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

		if (!Guid.TryParse(userIdClaim, out var userId))
			return Unauthorized();

		var user = await userRepository.FirstOrDefaultAsync(x => x.Id == userId, ct);

		if (user is null) {
			logger.LogWarning("Me endpoint — user {UserId} not found in database.", userId);
			return NotFound();
		}

		return Ok(new MeResponse(user.Id, user.Username, user.Email, user.CreatedAtUtc));
	}
}
