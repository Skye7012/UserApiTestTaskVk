using System;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using UserApiTestTaskVk.Domain.Entities;
using UserApiTestTaskVk.Domain.Exceptions;
using UserApiTestTaskVk.UnitTests.Mocks;
using Xunit;

namespace UserApiTestTaskVk.UnitTests.Entities;

/// <summary>
/// Тесты для <see cref="User"/>
/// </summary>
public class UserTests : UnitTestBase
{
	/// <summary>
	/// Должен выкинуть ошибку, когда указан пустой логин
	/// </summary>
	[Fact]
	public void Login_ShouldThrow_WhenEmptyLogin()
	{
		var context = CreateInMemoryContext();

		var act = () => new User(
				" ",
				new byte[] { 1, 2 },
				new byte[] { 1, 2 },
				context.DefaultUserGroup,
				context.ActiveUserState);

		act.Should()
			.Throw<ValidationProblem>()
			.WithMessage($"Поле {nameof(User.Login)} не может быть пустым");
	}

	/// <summary>
	/// Должен выкинуть ошибку, когда указан невалидный логин
	/// </summary>
	[Fact]
	public void Login_ShouldThrow_WhenInvalidLogin()
	{
		var context = CreateInMemoryContext();

		var act = () => new User(
				"Невалидный логин",
				new byte[] { 1, 2 },
				new byte[] { 1, 2 },
				context.DefaultUserGroup,
				context.ActiveUserState);

		act.Should()
			.Throw<ValidationProblem>()
			.WithMessage($"Для логина запрещены все символы кроме латинских букв и цифр");
	}

	/// <summary>
	/// Должен деактивировать все токены
	/// </summary>
	[Fact]
	public void RevokeAllRefreshTokens_ShouldRevokeAllTokens()
	{
		var createRefreshToken = (int n) => new RefreshToken(TokenServiceSubstitute.RefreshTokenName + n, AdminUser);

		var context = CreateInMemoryContext(x => 
			x.RefreshTokens.AddRange(Enumerable.Range(1, 5)
				.Select(n => createRefreshToken(n))));

		var adminUser = context.Users
			.Include(x => x.RefreshTokens)
			.First(x => x.Id == AdminUser.Id);

		adminUser.RevokeAllRefreshTokens();

		adminUser.RefreshTokens.Should().BeEmpty();
	}

	/// <summary>
	/// При добавлении шестого активного токена должен деактивировать самый старый активный токен
	/// </summary>
	[Fact]
	public void AddRefreshToken_ShouldRevokeExtraTokens_WhenAddingSixthActiveToken()
	{
		int i = 1;
		DateTimeProvider.UtcNow
			.Returns(x => DateTime.UtcNow.AddHours(i++));

		var createRefreshToken = (int n) => new RefreshToken(TokenServiceSubstitute.RefreshTokenName + n, AdminUser);

		var context = CreateInMemoryContext(x =>
		{
			x.RefreshTokens.AddRange(Enumerable.Range(1, 4)
				.Select(n => createRefreshToken(n)));

			x.RefreshTokens.Add(createRefreshToken(5));

			x.RefreshTokens.Add(new RefreshToken("RevokedToken", AdminUser)
			{
				RevokedOn = DateTime.UtcNow,
			});
		});

		var adminUser = context.Users
			.Include(x => x.RefreshTokens)
			.First(x => x.Id == AdminUser.Id);

		adminUser.AddRefreshToken(createRefreshToken(6));

		adminUser.RefreshTokens.Should()
			.NotContain(x => x.Token == TokenServiceSubstitute.RefreshTokenName + 5);

		adminUser.RefreshTokens.Should()
			.ContainSingle(x => x.Token == "RevokedToken");

		adminUser.RefreshTokens.Should()
			.ContainSingle(x => x.Token == TokenServiceSubstitute.RefreshTokenName + 6);
	}
}
