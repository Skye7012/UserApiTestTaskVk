using MediatR;
using UserApiTestTaskVk.Contracts.Requests.Authorization.PutUserLogin;

namespace UserApiTestTaskVk.Application.Authorization.Commands.PutUserLogin;

/// <summary>
/// Команда на обновление логина пользователя
/// </summary>
public class PutUserLoginCommand : PutUserLoginRequest, IRequest<PutUserLoginResponse>
{
	/// <summary>
	/// Конструктор
	/// </summary>
	/// <param name="id">Идентификатор</param>
	public PutUserLoginCommand(Guid id)
		=> Id = id;

	/// <summary>
	/// Идентификатор
	/// </summary>
	public Guid Id { get; set; }
}
