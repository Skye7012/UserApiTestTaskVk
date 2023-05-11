using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using UserApiTestTaskVk.Application.Authorization.Commands.SignIn;
using UserApiTestTaskVk.Application.Common.Exceptions;
using UserApiTestTaskVk.Domain.Entities;
using UserApiTestTaskVk.Domain.Exceptions;
using Xunit;

namespace UserApiTestTaskVk.UnitTests.Requests.AuthorizationRequests;

/// <summary>
/// Тест для <see cref="SignInCommandHandler"/>
/// </summary>
public class SignInCommandHandlerTests : UnitTestBase
{
	/// <summary>
	/// Должен создать пользователя, когда команда валидна
	/// </summary>
	[Fact]
	public async Task SignInCommand_ShouldCreateUser_WhenCommandValid()
	{
		var command = new SignInCommand
		{
			Login = AdminUser.Login,
			Password = AdminUser.Password,
		};

		using var context = CreateInMemoryContext();

		var accessToken = TokenService.CreateAccessToken(AdminUser);
		var refreshToken = TokenService.CreateRefreshToken();
		TokenService.ClearReceivedCalls();

		var handler = new SignInCommandHandler(
			context,
			TokenService,
			PasswordService);

		var response = await handler.Handle(command, default);

		response.AccessToken.Should().NotBeNullOrWhiteSpace();
		response.AccessToken.Should().Be(accessToken);

		response.RefreshToken.Should().NotBeNullOrWhiteSpace();
		response.RefreshToken.Should().Be(refreshToken);

		PasswordService.Received(1)
			.VerifyPasswordHash(command.Password, AdminUser.PasswordHash, AdminUser.PasswordSalt);

		TokenService.Received(1)
			.CreateRefreshToken();

		TokenService.Received(1)
			.CreateAccessToken(AdminUser);
	}

	/// <summary>
	/// Должен выкинуть ошибку, когда пользователь не найден
	/// </summary>
	[Fact]
	public async Task SignInQueryHandler_ShouldThrow_WhenUserNotFound()
	{
		var command = new SignInCommand
		{
			Login = AdminUser.Login,
			Password = AdminUser.Password,
		};

		using var context = CreateInMemoryContext(x => x.Users.Remove(AdminUser));

		var handler = new SignInCommandHandler(context, TokenService, PasswordService);
		var handle = async () => await handler.Handle(command, default);

		await handle.Should()
			.ThrowAsync<EntityNotFoundProblem<User>>();
	}

	/// <summary>
	/// Должен выкинуть ошибку, когда пользователь деактивирован
	/// </summary>
	[Fact]
	public async Task SignInQueryHandler_ShouldThrow_WhenUserBlocked()
	{
		var command = new SignInCommand
		{
			Login = AdminUser.Login,
			Password = AdminUser.Password,
		};

		using var context = CreateInMemoryContext();

		context.Users.Remove(AdminUser);
		context.SaveChanges();
		context.Instance.ChangeTracker.Clear();

		var handler = new SignInCommandHandler(context, TokenService, PasswordService);
		var handle = async () => await handler.Handle(command, default);

		await handle.Should()
			.ThrowAsync<ValidationProblem>()
			.WithMessage("Данные учетные данные были деактивированы");
	}

	/// <summary>
	/// Должен выкинуть ошибку, когда указан неправильный пароль
	/// </summary>
	[Fact]
	public async Task SignInQueryHandler_ShouldThrow_WhenPasswordIsWrong()
	{
		var command = new SignInCommand
		{
			Login = AdminUser.Login,
			Password = AdminUser.Password + "NotCorrectPassword",
		};

		using var context = CreateInMemoryContext();

		var handler = new SignInCommandHandler(context, TokenService, PasswordService);
		var handle = async () => await handler.Handle(command, default);

		await handle.Should()
			.ThrowAsync<ValidationProblem>()
			.WithMessage("Неправильный пароль");
	}
}
