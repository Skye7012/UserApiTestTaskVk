using MediatR;

namespace UserApiTestTaskVk.Application.Users.Commands.DeleteUser;

/// <summary>
/// Команда на удаление пользователя
/// </summary>
public class DeleteUserCommand : IRequest
{
	/// <summary>
	/// Конструктор
	/// </summary>
	/// <param name="id">Идентификатор</param>
	public DeleteUserCommand(Guid id)
		=> Id = id;

	/// <summary>
	/// Идентификатор
	/// </summary>
	public Guid Id { get; set; }
}
