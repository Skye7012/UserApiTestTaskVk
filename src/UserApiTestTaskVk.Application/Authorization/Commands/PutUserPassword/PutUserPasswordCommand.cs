using MediatR;
using UserApiTestTaskVk.Contracts.Requests.Authorization.PutUserPassword;

namespace UserApiTestTaskVk.Application.Authorization.Commands.PutUserPassword;

/// <summary>
/// Команда на обновление пароля пользователя
/// </summary>
public class PutUserPasswordCommand : PutUserPasswordRequest, IRequest<PutUserPasswordResponse>
{
	/// <summary>
	/// Конструктор
	/// </summary>
	/// <param name="id">Идентификатор</param>
	public PutUserPasswordCommand(Guid id)
		=> Id = id;

	/// <summary>
	/// Идентификатор
	/// </summary>
	public Guid Id { get; set; }
}
