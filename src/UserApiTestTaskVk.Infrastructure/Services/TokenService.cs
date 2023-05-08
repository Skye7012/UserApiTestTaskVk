using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using UserApiTestTaskVk.Application.Common.Exceptions;
using UserApiTestTaskVk.Application.Common.Interfaces;
using UserApiTestTaskVk.Application.Common.Static;
using UserApiTestTaskVk.Contracts.Common.Enums;
using UserApiTestTaskVk.Domain.Entities;
using UserApiTestTaskVk.Domain.Exceptions;
using UserApiTestTaskVk.Infrastructure.Configs;

namespace UserApiTestTaskVk.Infrastructure.Services;

/// <summary>
/// Сервис JWT токенов
/// </summary>
public class TokenService : ITokenService
{
	private readonly IApplicationDbContext _context;
	private readonly JwtConfig _jwtConfig;
	private readonly TokenValidationParameters _tokenValidationParameters;

	/// <summary>
	/// Конструктор
	/// </summary>
	/// <param name="dbContext">Контекст БД</param>
	/// <param name="jwtConfig">Конфигурация для JWT</param>
	/// <param name="tokenValidationParameters">Параметры валидации токена</param>
	public TokenService(
		IApplicationDbContext dbContext,
		IOptions<JwtConfig> jwtConfig,
		TokenValidationParameters tokenValidationParameters)
	{
		_context = dbContext;
		_jwtConfig = jwtConfig.Value;
		_tokenValidationParameters = tokenValidationParameters;
	}

	/// <inheritdoc/>
	public string CreateAccessToken(User user)
	{
		if (user.UserGroup == null)
			throw new NotIncludedProblem(nameof(UserGroup.Code));

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

	/// <inheritdoc/>
	public async Task<User> ValidateRefreshTokenAndReceiveUserAccount(
		string refreshToken,
		CancellationToken cancellationToken = default)
	{
		var receiveRefreshTokenAndRemoveFromDBAsync = async () =>
		{
			var refreshTokenEntity = await _context.RefreshTokens
				.Include(x => x.User)
				.FirstOrDefaultAsync(x => x.Token == refreshToken, cancellationToken)
				?? throw new EntityNotFoundProblem<RefreshToken>(
					nameof(RefreshToken.Token),
					refreshToken);

			_context.RefreshTokens.Remove(refreshTokenEntity);

			return refreshTokenEntity;
		};

		var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
		try
		{
			jwtSecurityTokenHandler.ValidateToken(refreshToken, _tokenValidationParameters, out var _);
			var refreshTokenEntity = await receiveRefreshTokenAndRemoveFromDBAsync();

			return refreshTokenEntity.RevokedOn != null
				? throw new ValidationProblem("Refresh токен не активный")
				: refreshTokenEntity.User!;
		}
		catch (Exception exception)
		{
			if (exception is SecurityTokenExpiredException expiredException)
				await receiveRefreshTokenAndRemoveFromDBAsync();

			throw new ValidationProblem("Refresh токен не валидный");
		}
	}
}
