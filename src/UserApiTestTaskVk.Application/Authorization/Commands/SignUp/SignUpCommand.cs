using MediatR;
using UserApiTestTaskVk.Contracts.Requests.Authorization.SignUp;

namespace UserApiTestTaskVk.Application.Authorization.Commands.SignUp;

/// <summary>
/// Команда для регистрации пользователя
/// </summary>
public class SignUpCommand : SignUpRequest, IRequest<SignUpResponse>
{
}
