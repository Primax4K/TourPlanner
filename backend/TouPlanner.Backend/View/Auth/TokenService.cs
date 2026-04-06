using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Model.Entities;

namespace View.Auth;

public interface ITokenService
{
	string CreateToken(ApplicationUser user);
}

public class TokenService : ITokenService {
	private readonly IConfiguration _config;

	public TokenService(IConfiguration config) {
		_config = config;
	}

	public string CreateToken(ApplicationUser user) {
		var jwtSettings = _config.GetSection("Jwt");
		var key = jwtSettings["Key"]!;
		var issuer = jwtSettings["Issuer"]!;
		var audience = jwtSettings["Audience"]!;
		var expiresMinutes = int.Parse(jwtSettings["ExpiresMinutes"]!);

		var claims = new List<Claim> {
			new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
			new(JwtRegisteredClaimNames.UniqueName, user.Username),
			new(JwtRegisteredClaimNames.Email, user.Email),
			new(ClaimTypes.Name, user.Username),
			new(ClaimTypes.Role, "user")
		};

		var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
		var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

		var token = new JwtSecurityToken(
			issuer: issuer,
			audience: audience,
			claims: claims,
			expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
			signingCredentials: creds
		);

		return new JwtSecurityTokenHandler().WriteToken(token);
	}
}