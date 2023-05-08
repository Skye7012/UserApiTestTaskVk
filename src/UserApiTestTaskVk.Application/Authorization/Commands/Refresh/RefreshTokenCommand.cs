using MediatR;
using UserApiTestTaskVk.Contracts.Requests.Authorization.Refresh;

namespace UserApiTestTaskVk.Application.Authorization.Commands.Refresh;

/// <summary>
/// Команда для обновления токена
/// </summary>
public class RefreshTokenCommand : RefreshTokenRequest, IRequest<RefreshTokenResponse>
{
}
