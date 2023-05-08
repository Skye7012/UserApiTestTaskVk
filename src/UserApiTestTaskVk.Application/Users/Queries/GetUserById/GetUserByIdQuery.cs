using MediatR;
using UserApiTestTaskVk.Contracts.Requests.Users.GetUser;

namespace UserApiTestTaskVk.Application.Users.Queries.GetUserById;

/// <summary>
/// Запрос на получение данных о пользователе
/// </summary>
public class GetUserByIdQuery : IRequest<GetUserResponse>
{
	/// <summary>
	/// Конструктор
	/// </summary>
	/// <param name="id">Идентификатор</param>
	public GetUserByIdQuery(Guid id)
		=> Id = id;

	/// <summary>
	/// Идентификатор
	/// </summary>
	public Guid Id { get; set; }
}
