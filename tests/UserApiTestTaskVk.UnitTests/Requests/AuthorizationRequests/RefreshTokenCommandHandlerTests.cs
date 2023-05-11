using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using UserApiTestTaskVk.Application.Authorization.Commands.Refresh;
using UserApiTestTaskVk.Domain.Entities;
using Xunit;

namespace UserApiTestTaskVk.UnitTests.Requests.AuthorizationRequests;

/// <summary>
/// Тест для <see cref="RefreshTokenCommandHandler"/>
/// </summary>
public class RefreshTokenCommandHandlerTests : UnitTestBase
{
	/// <summary>
	/// Должен обновить токены, когда команда валидна
	/// </summary>
	[Fact]
	public async Task RefreshTokenCommand_ShouldCreateNewTokens_WhenCommandValid()
	{
		var accessToken = TokenService.CreateAccessToken(AdminUser);
		var oldRefreshToken = "old" + TokenService.CreateRefreshToken();
		var command = new RefreshTokenCommand
		{
			RefreshToken = oldRefreshToken,
		};

		using var context = CreateInMemoryContext(x =>
			x.RefreshTokens.Add(new RefreshToken(oldRefreshToken, AdminUser)));

		TokenService.ClearReceivedCalls();

		var handler = new RefreshTokenCommandHandler(
			context,
			TokenService,
			RefreshTokenValidator);

		var response = await handler.Handle(command, default);

		response.AccessToken.Should().NotBeNullOrWhiteSpace();
		response.AccessToken.Should().Be(accessToken);

		response.RefreshToken.Should().NotBeNullOrWhiteSpace();
		response.RefreshToken.Should().NotBe(oldRefreshToken);

		context.Instance.ChangeTracker.Clear();
		var revokedRefreshToken = context.RefreshTokens
			.FirstOrDefault(x => x.Token == oldRefreshToken);

		revokedRefreshToken.Should().NotBeNull();
		revokedRefreshToken!.RevokedOn.Should().BeNull();

		var createdRefreshToken = context.RefreshTokens
			.FirstOrDefault(x => x.Token == response.RefreshToken);

		createdRefreshToken.Should().NotBeNull();
		createdRefreshToken!.RevokedOn.Should().BeNull();

		await RefreshTokenValidator.Received(1)
			.ValidateAndReceiveUserAsync(context, oldRefreshToken, default);

		TokenService.Received(1)
			.CreateRefreshToken();

		TokenService.Received(1)
			.CreateAccessToken(Arg.Is<User>(x => x.Id == AdminUser.Id));
	}
}
