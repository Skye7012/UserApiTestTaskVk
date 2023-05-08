using UserApiTestTaskVk.Contracts.Common.Enums;

namespace UserApiTestTaskVk.Contracts.Requests.Users.GetUser;

/// <summary>
/// Модель Группы пользователя для <see cref="GetUserResponse"/>
/// </summary>
public class GetUserGroupResponse
{
	/// <summary>
	/// Идентификатор
	/// </summary>
	public Guid Id { get; set; }

	/// <summary>
	/// Код
	/// </summary>
	public UserGroupCodes Code { get; set; } = default!;

	/// <summary>
	/// Описание
	/// </summary>
	public string Description { get; set; } = default!;
}
