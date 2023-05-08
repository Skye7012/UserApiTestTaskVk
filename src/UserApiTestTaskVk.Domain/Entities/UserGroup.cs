using UserApiTestTaskVk.Contracts.Common.Enums;
using UserApiTestTaskVk.Domain.Entities.Common;

namespace UserApiTestTaskVk.Domain.Entities;

/// <summary>
/// Группа пользователя
/// </summary>
public class UserGroup : EntityBase
{
	/// <summary>
	/// Код
	/// </summary>
	public UserGroupCodes Code { get; set; }

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
