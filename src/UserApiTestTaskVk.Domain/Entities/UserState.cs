using UserApiTestTaskVk.Contracts.Common.Enums;
using UserApiTestTaskVk.Domain.Entities.Common;

namespace UserApiTestTaskVk.Domain.Entities;

/// <summary>
/// Статус пользователя
/// </summary>
public class UserState : EntityBase
{
	/// <summary>
	/// Код
	/// </summary>
	public UserStateCodes Code { get; set; }

	/// <summary>
	/// Описание
	/// </summary>
	public string Description { get; set; } = default!;

	#region navigation Properties

	/// <summary>
	/// Пользователи
	/// </summary>
	public IReadOnlyList<User>? Users { get; set; }

	#endregion
}
