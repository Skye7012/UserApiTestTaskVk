using System;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using UserApiTestTaskVk.Application.Common.Exceptions;
using UserApiTestTaskVk.Application.Users.Queries.GetUserById;
using UserApiTestTaskVk.Domain.Entities;
using Xunit;

namespace UserApiTestTaskVk.UnitTests.Requests.UserRequests;

/// <summary>
/// Тест для <see cref="GetUserByIdQueryHandler"/>
/// </summary>
public class GetUserByIdQueryHandlerTests : UnitTestBase
{
	/// <summary>
	/// Должен получить информацию о пользователе, если он существует
	/// </summary>
	[Fact]
	public async Task GetUserByIdQueryHandler_ShouldReturnUserInfo_IfHeExists()
	{
		using var context = CreateInMemoryContext();
		var handler = new GetUserByIdQueryHandler(context, AuthorizationService);
		var result = await handler.Handle(new GetUserByIdQuery(AdminUser.Id), default);

		result.Should().NotBeNull();

		result.Id.Should().Be(AdminUser.Id);
		result.Login.Should().Be(AdminUser.Login);

		result.State.Id.Should().Be(AdminUser.UserState!.Id);
		result.State.Code.Should().Be(AdminUser.UserState!.Code);
		result.State.Description.Should().Be(AdminUser.UserState!.Description);

		result.Group.Id.Should().Be(AdminUser.UserGroup!.Id);
		result.Group.Code.Should().Be(AdminUser.UserGroup!.Code);
		result.Group.Description.Should().Be(AdminUser.UserGroup!.Description);

		AuthorizationService.Received(1)
			.CheckUserPermissionRule(AdminUser);
	}

	/// <summary>
	/// Должен выкинуть ошибку, когда пользователь не найден
	/// </summary>
	[Fact]
	public async Task GetUserByIdQueryHandler_ShouldThrow_WhenUserNotFound()
	{
		using var context = CreateInMemoryContext();

		var handler = new GetUserByIdQueryHandler(context, AuthorizationService);
		var handle = async () => await handler.Handle(new GetUserByIdQuery(Guid.NewGuid()), default);

		await handle.Should()
			.ThrowAsync<EntityNotFoundProblem<User>>();
	}
}
