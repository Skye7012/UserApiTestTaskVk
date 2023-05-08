using MediatR;
using UserApiTestTaskVk.Contracts.Requests.Authorization.SignIn;

namespace UserApiTestTaskVk.Application.Authorization.Commands.SignIn;

/// <summary>
/// Команда для авторизации пользователя
/// </summary>
public class SignInCommand : SignInRequest, IRequest<SignInResponse>
{
}
