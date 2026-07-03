using System.Security.Claims;
using Domain.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.Entities;
using View.Auth;
using View.Requests;
using View.Responses;

namespace View.Controllers.Auth;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase {
	private readonly ITokenService _tokenService;
	private readonly IApplicationUserRepository _userRepository;

	public AuthController(ITokenService tokenService, IApplicationUserRepository userRepository) {
		_tokenService = tokenService;
		_userRepository = userRepository;
	}

	[HttpPost("register")]
	public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request, CancellationToken ct) {
		var usernameExists = await _userRepository.ExistsAsync(x => x.Username == request.Username, ct);

		if (usernameExists) {
			return BadRequest("Username already exists.");
		}

		var emailExists = await _userRepository.ExistsAsync(x => x.Email == request.Email, ct);

		if (emailExists) {
			return BadRequest("Email already exists.");
		}

		var user = new ApplicationUser {
			Username = request.Username,
			Email = request.Email,
			PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
		};

		await _userRepository.CreateAsync(user, ct);

		var token = _tokenService.CreateToken(user);

		return Ok(new AuthResponse {
			Token = token,
			UserId = user.Id,
			Username = user.Username,
			Email = user.Email
		});
	}

	[HttpPost("login")]
	public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken ct) {
		var user = await _userRepository.FirstOrDefaultAsync(x =>
			x.Username == request.UsernameOrEmail || x.Email == request.UsernameOrEmail, ct);

		if (user is null) {
			return Unauthorized("Invalid credentials.");
		}

		var passwordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

		if (!passwordValid) {
			return Unauthorized("Invalid credentials.");
		}

		var token = _tokenService.CreateToken(user);

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

		var user = await _userRepository.FirstOrDefaultAsync(x => x.Id == userId, ct);

		if (user is null)
			return NotFound();

		return Ok(new MeResponse(user.Id, user.Username, user.Email, user.CreatedAtUtc));
	}
}