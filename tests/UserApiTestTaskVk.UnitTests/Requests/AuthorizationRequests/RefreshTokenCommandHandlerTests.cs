using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using UserApiTestTaskVk.Application.Authorization.Commands.Refresh;
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
		var refreshToken = TokenService.CreateRefreshToken();
		var command = new RefreshTokenCommand
		{
			RefreshToken = refreshToken,
		};

		using var context = CreateInMemoryContext();

		TokenService.ClearReceivedCalls();

		var handler = new RefreshTokenCommandHandler(
			context,
			TokenService);

		var response = await handler.Handle(command, default);

		response.AccessToken.Should().NotBeNullOrWhiteSpace();
		response.AccessToken.Should().Be(accessToken);

		response.RefreshToken.Should().NotBeNullOrWhiteSpace();
		response.RefreshToken.Should().Be(refreshToken);

		context.Instance.ChangeTracker.Clear();
		var createdRefreshToken = context.RefreshTokens
			.FirstOrDefault(x => x.Token == refreshToken);

		createdRefreshToken.Should().NotBeNull();
		createdRefreshToken!.RevokedOn.Should().BeNull();

		await TokenService.Received(1)
			.ValidateRefreshTokenAndReceiveUserAccountAsync(refreshToken, default);

		TokenService.Received(1)
			.CreateRefreshToken();

		TokenService.Received(1)
			.CreateAccessToken(AdminUser);
	}
}
