using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using UserApiTestTaskVk.Application.Common.Configs;
using UserApiTestTaskVk.Application.Common.Static;
using UserApiTestTaskVk.Infrastructure.Services;
using Xunit;

namespace UserApiTestTaskVk.UnitTests.Services;

/// <summary>
/// Тесты для <see cref="TokenService"/>
/// </summary>
public class TokenServiceTests : UnitTestBase
{
	private readonly TokenService _sut;
	private readonly JwtConfig _jwtConfig;

	/// <inheritdoc/>
	public TokenServiceTests()
	{
		CreateInMemoryContext();

		_jwtConfig = new JwtConfig()
		{
			Key = "JwtConfig key for unit tests",
			Audience = "Audience",
			Issuer = "Issuer",
		};

		var jwtConfigOptions = Options.Create(_jwtConfig);

		_sut = new TokenService(jwtConfigOptions);
	}

	/// <summary>
	/// Должен создать правильный JWT токен
	/// </summary>
	[Fact]
	public void CreateToken_ShoudCreateValidJwtToken()
	{
		var tokenString = _sut.CreateToken(default);

		var token = new JwtSecurityToken(tokenString);

		token.Issuer.Should().Be("Issuer");
		token.Audiences.Single().Should().Be("Audience");
		token.SignatureAlgorithm.Should().Be(SecurityAlgorithms.HmacSha512Signature);
	}

	/// <summary>
	/// Должен создать правильный Access токен с правильными клеймами
	/// </summary>
	[Fact]
	public void CreateAccessToken_ShoudCreateValidAccessToken()
	{
		var tokenString = _sut.CreateAccessToken(AdminUser);

		var token = new JwtSecurityToken(tokenString);

		var userIdClaimValue = token.Claims
			.First(x => x.Type == CustomClaims.UserIdСlaimName)
			.Value;

		userIdClaimValue.Should().Be(AdminUser.Id.ToString());

		var isAdminClaimValue = bool.Parse(token.Claims
			.First(x => x.Type == CustomClaims.IsAdminClaimName)
			.Value);

		token.ValidTo.Should().BeCloseTo(
			DateTime.UtcNow.AddSeconds(_jwtConfig.AccessTokenLifeTime),
			TimeSpan.FromSeconds(1));
	}

	/// <summary>
	/// Должен создать правильный Refresh токен с правильным lifetime
	/// </summary>
	[Fact]
	public void CreateRefreshToken_ShoudCreateValidRefreshToken()
	{
		var tokenString = _sut.CreateRefreshToken();

		var token = new JwtSecurityToken(tokenString);

		token.ValidTo.Should().BeCloseTo(
			DateTime.UtcNow.AddSeconds(_jwtConfig.RefreshTokenLifeTime),
			TimeSpan.FromSeconds(1));
	}
}
