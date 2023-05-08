using UserApiTestTaskVk.Contracts.Common.Enums;

namespace UserApiTestTaskVk.Contracts.Requests.Users.GetUser;

/// <summary>
/// Модель Статуса пользователя для <see cref="GetUserResponse"/>
/// </summary>
public class GetUserStateResponse
{
	/// <summary>
	/// Идентификатор
	/// </summary>
	public Guid Id { get; set; }

	/// <summary>
	/// Код
	/// </summary>
	public UserStateCodes Code { get; set; } = default!;

	/// <summary>
	/// Описание
	/// </summary>
	public string Description { get; set; } = default!;
}
