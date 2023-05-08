namespace UserApiTestTaskVk.Contracts.Requests.Users.GetUser;

/// <summary>
/// Ответ на запрос получения данных о пользователе
/// </summary>
public class GetUserResponse
{
	/// <summary>
	/// Идентификатор
	/// </summary>
	public Guid Id { get; set; }

	/// <summary>
	/// Логин
	/// </summary>
	public string Login { get; set; } = default!;

	/// <summary>
	/// Группа пользователя
	/// </summary>
	public GetUserGroupResponse Group { get; set; } = default!;

	/// <summary>
	/// Статус пользователя
	/// </summary>
	public GetUserStateResponse State { get; set; } = default!;
}
