using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using UserApiTestTaskVk.Application.Common.Configs;
using UserApiTestTaskVk.Application.Common.Interfaces;
using UserApiTestTaskVk.Application.Common.Static;
using UserApiTestTaskVk.Contracts.Common.Enums;
using UserApiTestTaskVk.Domain.Entities;
using UserApiTestTaskVk.Domain.Exceptions;

namespace UserApiTestTaskVk.Infrastructure.Services;

/// <summary>
/// Сервис JWT токенов
/// </summary>
public class TokenService : ITokenService
{
	private readonly JwtConfig _jwtConfig;

	/// <summary>
	/// Конструктор
	/// </summary>
	/// <param name="jwtConfig">Конфигурация для JWT</param>
	public TokenService(IOptions<JwtConfig> jwtConfig)
		=> _jwtConfig = jwtConfig.Value;

	/// <inheritdoc/>
	public string CreateAccessToken(User user)
	{
		if (user.UserGroup == null)
			throw new NotIncludedProblem(nameof(User.UserGroup));

		List<Claim> claims = new()
		{
			new Claim(CustomClaims.UserIdСlaimName, user.Id.ToString()),
			new Claim(
				CustomClaims.IsAdminClaimName,
				(user.UserGroup!.Code == UserGroupCodes.Admin).ToString()),
		};

		return CreateToken(
			DateTime.UtcNow.AddSeconds(_jwtConfig.AccessTokenLifeTime),
			claims);
	}

	/// <inheritdoc/>
	public string CreateRefreshToken()
		=> CreateToken(DateTime.UtcNow.AddSeconds(_jwtConfig.RefreshTokenLifeTime));

	/// <inheritdoc/>
	public string CreateToken(DateTime expires, IEnumerable<Claim>? claims = null)
	{
		var key = new SymmetricSecurityKey(
			System.Text.Encoding.UTF8.GetBytes(_jwtConfig.Key));

		var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

		var token = new JwtSecurityToken(
			claims: claims,
			expires: expires,
			issuer: _jwtConfig.Issuer,
			audience: _jwtConfig.Audience,
			signingCredentials: creds);

		var jwt = new JwtSecurityTokenHandler().WriteToken(token);

		return jwt;
	}
}
