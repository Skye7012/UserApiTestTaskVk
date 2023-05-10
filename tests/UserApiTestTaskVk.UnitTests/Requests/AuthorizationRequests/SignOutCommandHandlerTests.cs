using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using UserApiTestTaskVk.Application.Authorization.Commands.SignOut;
using Xunit;

namespace UserApiTestTaskVk.UnitTests.Requests.AuthorizationRequests;

/// <summary>
/// Тест для <see cref="SignOutCommandHandler"/>
/// </summary>
public class SignOutCommandHandlerTests : UnitTestBase
{
	/// <summary>
	/// Должен деактивировать все refresh токены пользователя, когда комманда валидна
	/// </summary>
	[Fact]
	public async Task SignOutCommand_ShouldSoftDeleteAllRefreshTokens_WhenCommandValid()
	{
		var accessToken = TokenService.CreateAccessToken(AdminUser);
		var refreshToken = TokenService.CreateRefreshToken();

		using var context = CreateInMemoryContext();

		TokenService.ClearReceivedCalls();

		var handler = new SignOutCommandHandler(
			context,
			AuthorizationService);

		await handler.Handle(new SignOutCommand(), default);

		context.Instance.ChangeTracker.Clear();
		var refreshTokensWereSoftDeleted = context.RefreshTokens
			.Where(x => x.UserId == AdminUser.Id)
			.All(x => x.RevokedOn != null);

		refreshTokensWereSoftDeleted.Should().BeTrue();

		AuthorizationService.Received(1)
			.GetUserId();
	}
}
