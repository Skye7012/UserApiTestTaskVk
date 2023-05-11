using System.Collections.Generic;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using UserApiTestTaskVk.Application.Common.Exceptions;
using UserApiTestTaskVk.Application.Common.Static;
using UserApiTestTaskVk.Contracts.Common.Enums;
using UserApiTestTaskVk.Domain.Entities;
using UserApiTestTaskVk.Infrastructure.Services;
using Xunit;

namespace UserApiTestTaskVk.UnitTests.Services;

/// <summary>
/// Тесты для <see cref="AuthorizationService"/>
/// </summary>
public class AuthorizationServiceTests : UnitTestBase
{
	private readonly AuthorizationService _sut;

	/// <inheritdoc/>
	public AuthorizationServiceTests()
	{
		var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
		var context = new DefaultHttpContext();
		var claims = new List<Claim>
		{
			new Claim(CustomClaims.UserIdСlaimName, AdminUser.Id.ToString()),
			new Claim(CustomClaims.IsAdminClaimName, true.ToString()),
		};
		context.User.AddIdentity(new ClaimsIdentity(claims));
		httpContextAccessor.HttpContext
			.Returns(context);

		_sut = new AuthorizationService(httpContextAccessor);
	}

	/// <summary>
	/// Сформировать тестируемый сервис
	/// </summary>
	/// <param name="claims">Клеймы</param>
	/// <param name="user">Пользователь</param>
	/// <returns>Тестируемый сервис</returns>
	private AuthorizationService BuildSut(
		List<Claim>? claims = null,
		User? user = null)
	{
		user ??= AdminUser;
		claims ??= new List<Claim>
		{
			new Claim(CustomClaims.UserIdСlaimName, user.Id.ToString()),
			new Claim(
				CustomClaims.IsAdminClaimName,
				(user.UserGroup!.Code == UserGroupCodes.Admin).ToString()),
		};

		var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
		var context = new DefaultHttpContext();
		context.User.AddIdentity(new ClaimsIdentity(claims));
		httpContextAccessor.HttpContext
			.Returns(context);

		return new AuthorizationService(httpContextAccessor);
	}

	/// <summary>
	/// Сформировать тестируемый сервис
	/// </summary>
	/// <param name="user">Пользователь</param>
	/// <returns>Тестируемый сервис</returns>
	private AuthorizationService BuildSut(User user)
		=> BuildSut(null, user);

	/// <summary>
	/// Все методы должны правильно вызываться с правильными клеймами пользователя-администратора <br/>
	/// ultimate happy path
	/// </summary>
	[Fact]
	public void AllMethods_ShouldWork_WithValidClaimsOfAdminUser()
	{
		_sut.IsAuthenticated().Should().BeTrue();
		_sut.IsAdmin().Should().BeTrue();
		_sut.GetUserId().Should().Be(AdminUser.Id);

		_sut.CheckUserPermissionRule(AdminUser);
	}

	/// <summary>
	/// Должен выкинуть ошибку при невалидных клеймах
	/// </summary>
	[Fact]
	public void IsAuthenticated_ShouldThrow_WhenInvalidClaims()
	{
		var sut = BuildSut(new List<Claim>());
		sut.IsAuthenticated().Should().BeFalse();
	}

	/// <summary>
	/// Должен выкинуть ошибку при невалидном клейме
	/// </summary>
	[Fact]
	public void IsAdmin_ShouldThrow_WhenInvalidClaim()
	{
		var sut = BuildSut(new List<Claim>()
		{
			new Claim(CustomClaims.IsAdminClaimName, "Invalid")
		});

		var act = () => sut.IsAdmin();

		act.Should()
			.Throw<UnauthorizedProblem>()
			.WithMessage($"Невалидный клейм '{CustomClaims.IsAdminClaimName}' " +
				$"в токене");
	}

	/// <summary>
	/// Должен выкинуть ошибку при невалидном клейме
	/// </summary>
	[Fact]
	public void GetUserId_ShouldThrow_WhenInvalidClaim()
	{
		var sut = BuildSut(new List<Claim>()
		{
			new Claim(CustomClaims.UserIdСlaimName, "Invalid")
		});

		var act = () => sut.GetUserId();

		act.Should()
			.Throw<UnauthorizedProblem>()
			.WithMessage($"Невалидный клейм '{CustomClaims.UserIdСlaimName}' " +
				$"в токене");
	}

	/// <summary>
	/// Должен пройти проверку при передаче правильного пользователя
	/// </summary>
	[Fact]
	public void CheckUserPermissionRule_ShouldPass_WhenValidUser()
	{
		var context = CreateInMemoryContext();

		var user = new User(
				"new",
				new byte[] { 1, 2 },
				new byte[] { 1, 2 },
				context.DefaultUserGroup,
				context.ActiveUserState);

		var sut = BuildSut(user);

		sut.CheckUserPermissionRule(user);
	}

	/// <summary>
	/// Должен выкинуть ошибку при передаче правильного пользователя
	/// </summary>
	[Fact]
	public void CheckUserPermissionRule_ShouldThrow_WhenInvalidUser()
	{
		var context = CreateInMemoryContext();

		var user = new User(
				"new",
				new byte[] { 1, 2 },
				new byte[] { 1, 2 },
				context.DefaultUserGroup,
				context.ActiveUserState);

		var sut = BuildSut(
			new List<Claim>()
			{
				new Claim(CustomClaims.UserIdСlaimName, AdminUser.Id.ToString()),
				new Claim(CustomClaims.IsAdminClaimName, false.ToString())
			},
			user);

		var act = () => sut.CheckUserPermissionRule(user);

		act.Should()
			.Throw<ForbiddenProblem>()
			.WithMessage("Данное действие доступно администратору, " +
				"либо лично пользователю, если он активен");
	}

	/// <summary>
	/// Должен выкинуть ошибку при передаче неактивного пользователя
	/// </summary>
	[Fact]
	public void CheckUserPermissionRule_ShouldThrow_WhenBlockedUser()
	{
		var context = CreateInMemoryContext();

		var user = new User(
				"new",
				new byte[] { 1, 2 },
				new byte[] { 1, 2 },
				context.DefaultUserGroup,
				context.BlockedUserState);

		var sut = BuildSut(user);

		var act = () => sut.CheckUserPermissionRule(user);

		act.Should()
			.Throw<ForbiddenProblem>()
			.WithMessage("Данное действие доступно администратору, " +
				"либо лично пользователю, если он активен");
	}
}
