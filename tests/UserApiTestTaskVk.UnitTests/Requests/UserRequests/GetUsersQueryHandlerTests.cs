using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using UserApiTestTaskVk.Application.Users.Queries.GetUsers;
using UserApiTestTaskVk.Domain.Entities;
using Xunit;

namespace UserApiTestTaskVk.UnitTests.Requests.UserRequests;

/// <summary>
/// Тест для <see cref="GetUsersQueryHandler"/>
/// </summary>
public class GetUsersQueryHandlerTests : UnitTestBase
{
	/// <summary>
	/// Должен получить информацию о пользователях, если они существует
	/// </summary>
	[Fact]
	public async Task GetUsersQueryHandler_ShouldReturnUserInfo_IfTheyExists()
	{
		using var context = CreateInMemoryContext(x => 
			x.Users.Add(new User(
				"new",
				new byte[] { 1, 2 },
				new byte[] { 1, 2 },
				x.DefaultUserGroup,
				x.ActiveUserState)));

		var handler = new GetUsersQueryHandler(context);
		var result = await handler.Handle(new GetUsersQuery(), default);

		result.Should().NotBeNull();
		result.TotalCount.Should().Be(2);

		result.Items.Should().NotBeNullOrEmpty();
		var adminUser = result.Items!
			.First(x => x.Id == AdminUser.Id);

		adminUser.Id.Should().Be(AdminUser.Id);
		adminUser.Login.Should().Be(AdminUser.Login);

		adminUser.State.Id.Should().Be(AdminUser.UserState!.Id);
		adminUser.State.Code.Should().Be(AdminUser.UserState!.Code);
		adminUser.State.Description.Should().Be(AdminUser.UserState!.Description);

		adminUser.Group.Id.Should().Be(AdminUser.UserGroup!.Id);
		adminUser.Group.Code.Should().Be(AdminUser.UserGroup!.Code);
		adminUser.Group.Description.Should().Be(AdminUser.UserGroup!.Description);
	}
}
