using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using UserApiTestTaskVk.Application.Common.Exceptions;
using UserApiTestTaskVk.Application.Users.Commands.DeleteUser;
using UserApiTestTaskVk.Domain.Entities;
using Xunit;

namespace UserApiTestTaskVk.UnitTests.Requests.UserRequests;

/// <summary>
/// Тест для <see cref="DeleteUserCommandHandler"/>
/// </summary>
public class DeleteUserCommandHandlerTests : UnitTestBase
{
	/// <summary>
	/// Должен удалить пользователя, когда команда валидна
	/// </summary>
	[Fact]
	public async Task DeleteUserCommand_ShouldCreateUser_WhenCommandValid()
	{
		using var context = CreateInMemoryContext();

		var handler = new DeleteUserCommandHandler(context, AuthorizationService);
		await handler.Handle(new DeleteUserCommand(AdminUser.Id), default);

		var adminUser = context.Users
			.FirstOrDefault(x => x.Id == AdminUser.Id);

		adminUser.Should().NotBeNull();
		adminUser!.UserState.Should().Be(context.BlockedUserState);

		AuthorizationService.Received(1)
			.CheckUserPermissionRule(AdminUser);
	}

	/// <summary>
	/// Должен выкинуть ошибку, когда пользователь не найден
	/// </summary>
	[Fact]
	public async Task DeleteUserCommand_ShouldThrow_WhenUserNotFound()
	{
		using var context = CreateInMemoryContext();

		var handler = new DeleteUserCommandHandler(context, AuthorizationService);
		var handle = async () => await handler.Handle(new DeleteUserCommand(Guid.NewGuid()), default);

		await handle.Should()
			.ThrowAsync<EntityNotFoundProblem<User>>();
	}
}
