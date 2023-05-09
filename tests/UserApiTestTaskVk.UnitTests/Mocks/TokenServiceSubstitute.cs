using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using UserApiTestTaskVk.Application.Common.Interfaces;
using UserApiTestTaskVk.Domain.Entities;
using UserApiTestTaskVk.Domain.Exceptions;
using UserApiTestTaskVk.UnitTests.Common.Interfaces;

namespace UserApiTestTaskVk.UnitTests.Mocks;

/// <summary>
/// Сервис JWT токенов для тестов
/// </summary>
public class TokenServiceSubstitute : ITokenService, ISubstitute<ITokenService>
{
	public const string AccessTokenName = "accessToken";
	public const string RefreshTokenName = "refreshToken";

	private readonly User _adminUser;

	/// <summary>
	/// Конструктор
	/// </summary>
	/// <param name="adminUser">Пользователь-администратор</param>
	public TokenServiceSubstitute(User adminUser)
		=> _adminUser = adminUser;

	/// <inheritdoc/>
	public ITokenService Create()
		=> Substitute.ForPartsOf<TokenServiceSubstitute>(_adminUser);

	/// <inheritdoc/>
	public virtual string CreateAccessToken(User userAccount)
		=> AccessTokenName;

	/// <inheritdoc/>
	public virtual string CreateRefreshToken()
		=> RefreshTokenName;

	/// <inheritdoc/>
	public virtual string CreateToken(DateTime expires, IEnumerable<Claim>? claims = null)
		=> "token";

	/// <inheritdoc/>
	public virtual Task<User> ValidateRefreshTokenAndReceiveUserAccountAsync(string refreshToken, CancellationToken cancellationToken = default)
		=> refreshToken == RefreshTokenName
			? Task.FromResult(_adminUser)
			: throw new ValidationProblem("ValidateRefreshTokenAndReceiveUserAccount");
}
