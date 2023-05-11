using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using UserApiTestTaskVk.Application.Authorization.Commands.SignOut;
using UserApiTestTaskVk.Domain.Entities;
using Xunit;

namespace UserApiTestTaskVk.UnitTests.Requests.AuthorizationRequests;

/// <summary>
/// Тест для <see cref="SignOutCommandHandler"/>
/// </summary>
public class SignOutCommandHandlerTests : UnitTestBase
{
	/// <summary>
	/// Должен деактивировать все refresh токены пользователя, когда команда валидна
	/// </summary>
	[Fact]
	public async Task SignOutCommand_ShouldSoftDeleteAllRefreshTokens_WhenCommandValid()
	{
		var refreshToken = TokenService.CreateRefreshToken();

		using var context = CreateInMemoryContext(x =>
		{
			x.RefreshTokens.Add(new RefreshToken(refreshToken + "1", AdminUser));
			x.RefreshTokens.Add(new RefreshToken(refreshToken + "2", AdminUser));
		});

		var handler = new SignOutCommandHandler(
			context,
			AuthorizationService);

		await handler.Handle(new SignOutCommand(), default);

		context.Instance.ChangeTracker.Clear();

		var refreshTokenCount = context.RefreshTokens
			.Where(x => x.UserId == AdminUser.Id)
			.Count();

		refreshTokenCount.Should().Be(2);

		var refreshTokensWereSoftDeleted = context.RefreshTokens
			.Where(x => x.UserId == AdminUser.Id)
			.All(x => x.RevokedOn != null);

		refreshTokensWereSoftDeleted.Should().BeTrue();

		AuthorizationService.Received(1)
			.GetUserId();
	}
}
