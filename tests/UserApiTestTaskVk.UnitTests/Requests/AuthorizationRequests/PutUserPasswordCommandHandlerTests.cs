using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using UserApiTestTaskVk.Application.Authorization.Commands.PutUserPassword;
using UserApiTestTaskVk.Application.Common.Exceptions;
using UserApiTestTaskVk.Domain.Entities;
using UserApiTestTaskVk.Domain.Exceptions;
using Xunit;

namespace UserApiTestTaskVk.UnitTests.Requests.AuthorizationRequests;

/// <summary>
/// Тест для <see cref="PutUserPasswordCommandHandler"/>
/// </summary>
public class PutUserPasswordCommandHandlerTests : UnitTestBase
{
	/// <summary>
	/// Должен изменить пароль пользователя, когда команда валидна
	/// </summary>
	[Fact]
	public async Task PutUserPasswordCommand_ShouldCreateUser_WhenCommandValid()
	{
		var accessToken = TokenService.CreateAccessToken(AdminUser);
		var refreshToken = TokenService.CreateRefreshToken();

		using var context = CreateInMemoryContext(x =>
		{
			x.RefreshTokens.Add(new RefreshToken(refreshToken + "1", AdminUser));
			x.RefreshTokens.Add(new RefreshToken(refreshToken + "2", AdminUser));
		});

		TokenService.ClearReceivedCalls();

		var command = new PutUserPasswordCommand(AdminUser.Id)
		{
			OldPassword = AdminUser.Password,
			NewPassword = "NewAdminPassword",
		};

		var handler = new PutUserPasswordCommandHandler(
			context,
			AuthorizationService,
			PasswordService,
			TokenService);

		var response = await handler.Handle(command, default);

		context.Instance.ChangeTracker.Clear();
		var updatedUser = context.Users
			.Include(x => x.RefreshTokens)
			.FirstOrDefault(x => x.Id == AdminUser.Id);

		updatedUser.Should().NotBeNull();

		PasswordService.Received(1)
			.VerifyPasswordHash(command.OldPassword, AdminUser.PasswordHash, AdminUser.PasswordSalt);

		PasswordService.VerifyPasswordHash(
				command.NewPassword,
				updatedUser!.PasswordHash,
				updatedUser!.PasswordSalt)
			.Should().BeTrue();

		updatedUser.RefreshTokens.Should().NotBeNullOrEmpty();
		updatedUser.RefreshTokens.Should().ContainSingle(x => x.RevokedOn == null);

		AuthorizationService.Received(1)
			.CheckUserPermissionRule(Arg.Is<User>(u => u.Id == AdminUser.Id));

		PasswordService.Received(1)
			.CreatePasswordHash(
				command.NewPassword,
				out Arg.Is<byte[]>(x => x.SequenceEqual(updatedUser.PasswordHash)),
				out Arg.Is<byte[]>(x => x.SequenceEqual(updatedUser.PasswordSalt)));

		TokenService.Received(1)
			.CreateRefreshToken();

		TokenService.Received(1)
			.CreateAccessToken(Arg.Is<User>(u => u.Id == AdminUser.Id));
	}

	/// <summary>
	/// Должен выкинуть ошибку, когда пользователь не найден
	/// </summary>
	[Fact]
	public async Task PutUserPasswordCommand_ShouldThrow_WhenUserNotFound()
	{
		using var context = CreateInMemoryContext();

		var command = new PutUserPasswordCommand(Guid.NewGuid())
		{
			OldPassword = AdminUser.Password,
			NewPassword = "NewPassword",
		};

		var handler = new PutUserPasswordCommandHandler(context, AuthorizationService, PasswordService, TokenService);
		var handle = async () => await handler.Handle(command, default);

		await handle.Should()
			.ThrowAsync<EntityNotFoundProblem<User>>();
	}

	/// <summary>
	/// Должен выкинуть ошибку, когда введен не правильный текущий пароль
	/// </summary>
	[Fact]
	public async Task PutUserPasswordCommand_ShouldThrow_WhenOldPasswordIsWrong()
	{
		using var context = CreateInMemoryContext();

		var command = new PutUserPasswordCommand(AdminUser.Id)
		{
			OldPassword = AdminUser.Password + "Wrong",
			NewPassword = "NewPassword",
		};

		var handler = new PutUserPasswordCommandHandler(context, AuthorizationService, PasswordService, TokenService);
		var handle = async () => await handler.Handle(command, default);

		await handle.Should()
			.ThrowAsync<ValidationProblem>("Введен неверный текущий пароль пользователя");
	}

	/// <summary>
	/// Должен выкинуть ошибку, когда указан невалидный пароль
	/// </summary>
	[Fact]
	public async Task SignUpQueryHandler_ShouldThrow_WhenPasswordIsNotValid()
	{
		using var context = CreateInMemoryContext();

		var command = new PutUserPasswordCommand(AdminUser.Id)
		{
			OldPassword = AdminUser.Password,
			NewPassword = "Невалидный пароль",
		};

		var handler = new PutUserPasswordCommandHandler(context, AuthorizationService, PasswordService, TokenService);
		var handle = async () => await handler.Handle(command, default);

		await handle.Should()
			.ThrowAsync<ValidationProblem>()
			.WithMessage("Для пароля запрещены все символы кроме латинских букв и цифр");
	}
}
