using System;
using System.Collections.Generic;
using System.Security.Claims;
using NSubstitute;
using UserApiTestTaskVk.Application.Common.Interfaces;
using UserApiTestTaskVk.Domain.Entities;
using UserApiTestTaskVk.UnitTests.Common.Interfaces;

namespace UserApiTestTaskVk.UnitTests.Mocks;

/// <summary>
/// Сервис JWT токенов для тестов
/// </summary>
public class TokenServiceSubstitute : ITokenService, ISubstitute<ITokenService>
{
	public const string AccessTokenName = "accessToken";
	public const string RefreshTokenName = "refreshToken";

	/// <inheritdoc/>
	public ITokenService Create()
		=> Substitute.ForPartsOf<TokenServiceSubstitute>();

	/// <inheritdoc/>
	public virtual string CreateAccessToken(User user)
		=> AccessTokenName;

	/// <inheritdoc/>
	public virtual string CreateRefreshToken()
		=> RefreshTokenName;

	/// <inheritdoc/>
	public virtual string CreateToken(DateTime expires, IEnumerable<Claim>? claims = null)
		=> "token";
}
