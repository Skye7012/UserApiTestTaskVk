using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using UserApiTestTaskVk.Application.Authorization.Commands.PutUserLogin;
using UserApiTestTaskVk.Application.Common.Exceptions;
using UserApiTestTaskVk.Domain.Entities;
using UserApiTestTaskVk.Domain.Exceptions;
using Xunit;

namespace UserApiTestTaskVk.UnitTests.Requests.AuthorizationRequests;

/// <summary>
/// Тест для <see cref="PutUserLoginCommandHandler"/>
/// </summary>
public class PutUserLoginCommandHandlerTests : UnitTestBase
{
	/// <summary>
	/// Должен изменить логин пользователя, когда команда валидна
	/// </summary>
	[Fact]
	public async Task PutUserLoginCommand_ShouldCreateUser_WhenCommandValid()
	{
		var accessToken = TokenService.CreateAccessToken(AdminUser);
		var refreshToken = TokenService.CreateRefreshToken();

		using var context = CreateInMemoryContext(x =>
		{
			x.RefreshTokens.Add(new RefreshToken(refreshToken, AdminUser));
			x.RefreshTokens.Add(new RefreshToken(refreshToken, AdminUser));
			x.SaveChanges();
			x.Instance.ChangeTracker.Clear();
		});

		TokenService.ClearReceivedCalls();

		var command = new PutUserLoginCommand(AdminUser.Id)
		{
			NewLogin = "NewAdmin",
		};

		var handler = new PutUserLoginCommandHandler(
			context,
			AuthorizationService,
			TokenService);

		var response = await handler.Handle(command, default);

		context.Instance.ChangeTracker.Clear();
		var updatedUser = context.Users
			.Include(x => x.RefreshTokens)
			.FirstOrDefault(x => x.Id == AdminUser.Id);

		updatedUser.Should().NotBeNull();
		updatedUser!.Login.Should().Be(command.NewLogin);

		updatedUser.RefreshTokens.Should().NotBeNullOrEmpty();
		updatedUser.RefreshTokens.Should().ContainSingle(x => x.RevokedOn == null);

		AuthorizationService.Received(1)
			.CheckUserPermissionRule(Arg.Is<User>(u => u.Id == AdminUser.Id));

		TokenService.Received(1)
			.CreateRefreshToken();

		TokenService.Received(1)
			.CreateAccessToken(Arg.Is<User>(u => u.Id == AdminUser.Id));
	}

	/// <summary>
	/// Должен выкинуть ошибку, когда пользователь не найден
	/// </summary>
	[Fact]
	public async Task PutUserLoginCommand_ShouldThrow_WhenUserNotFound()
	{
		using var context = CreateInMemoryContext();

		var command = new PutUserLoginCommand(Guid.NewGuid())
		{
			NewLogin = "NewAdmin",
		};

		var handler = new PutUserLoginCommandHandler(context, AuthorizationService, TokenService);
		var handle = async () => await handler.Handle(command, default);

		await handle.Should()
			.ThrowAsync<EntityNotFoundProblem<User>>();
	}

	/// <summary>
	/// Должен выкинуть ошибку, когда пользователь с таким логином уже существует
	/// </summary>
	[Fact]
	public async Task PutUserLoginCommand_ShouldThrow_WhenNewLoginIsNotUnique()
	{
		using var context = CreateInMemoryContext(x =>
		{
			x.Users.Add(new User(
					"new",
					new byte[] { 1, 2 },
					new byte[] { 1, 2 },
					x.DefaultUserGroup,
					x.ActiveUserState));

			x.SaveChanges();
			x.Instance.ChangeTracker.Clear();
		});

		var command = new PutUserLoginCommand(AdminUser.Id)
		{
			NewLogin = "new",
		};

		var handler = new PutUserLoginCommandHandler(context, AuthorizationService, TokenService);
		var handle = async () => await handler.Handle(command, default);

		await handle.Should()
			.ThrowAsync<ValidationProblem>("Пользователь с таким логином уже существует, " +
				"новый логин должен быть уникальным");
	}
}
