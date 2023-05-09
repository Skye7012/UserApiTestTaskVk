using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using UserApiTestTaskVk.Application.Authorization.Commands.SignUp;
using UserApiTestTaskVk.Domain.Entities;
using UserApiTestTaskVk.Domain.Exceptions;
using Xunit;

namespace UserApiTestTaskVk.UnitTests.Requests.AuthorizationRequests;

/// <summary>
/// Тест для <see cref="SignUpCommandHandler"/>
/// </summary>
public class SignUpCommandHandlerTests : UnitTestBase
{
	/// <summary>
	/// Должен создать пользователя, когда команда валидна
	/// </summary>
	[Fact]
	public async Task SignUpCommand_ShouldCreateUser_WhenCommandValid()
	{
		var command = new SignUpCommand
		{
			Login = "testlogin",
			Password = "testpassword",
		};

		using var context = CreateInMemoryContext();

		var handler = new SignUpCommandHandler(
			context,
			PasswordService,
			DistributedLockProvider,
			LockDelaysConfig);

		var response = await handler.Handle(command, default);

		var createdUser = context.Users.FirstOrDefault(x => x.Id == response.UserId);

		createdUser.Should().NotBeNull();
		createdUser!.Login.Should().Be(command.Login);

		PasswordService.VerifyPasswordHash(
				command.Password,
				createdUser!.PasswordHash,
				createdUser!.PasswordSalt)
			.Should().BeTrue();

		DistributedLockProvider.Received(1)
			.CreateLock($"{User.GetLockKey(command.Login)}");

		PasswordService.Received(1)
			.CreatePasswordHash(command.Password, out Arg.Any<byte[]>(), out Arg.Any<byte[]>());
	}

	/// <summary>
	/// Должен выкинуть ошибку, когда указан неуникальный логин
	/// </summary>
	[Fact]
	public async Task SignUpQueryHandler_ShouldThrow_WhenLoginIsNotUnique()
	{
		var command = new SignUpCommand
		{
			Login = AdminUser.Login,
			Password = AdminUser.Password,
		};

		using var context = CreateInMemoryContext();

		var handler = new SignUpCommandHandler(
			context,
			PasswordService,
			DistributedLockProvider,
			LockDelaysConfig);

		var handle = async () => await handler.Handle(command, default);

		await handle.Should()
			.ThrowAsync<ValidationProblem>()
			.WithMessage("Пользователь с таким логином уже существует");
	}

	/// <summary>
	/// Должен выкинуть ошибку, когда указан невалидный пароль
	/// </summary>
	[Fact]
	public async Task SignUpQueryHandler_ShouldThrow_WhenPasswordIsNotValid()
	{
		var command = new SignUpCommand
		{
			Login = "newUser",
			Password = "Невалидный пароль",
		};

		using var context = CreateInMemoryContext();

		var handler = new SignUpCommandHandler(
			context,
			PasswordService,
			DistributedLockProvider,
			LockDelaysConfig);

		var handle = async () => await handler.Handle(command, default);

		await handle.Should()
			.ThrowAsync<ValidationProblem>()
			.WithMessage("Для пароля запрещены все символы кроме латинских букв и цифр");
	}
}
