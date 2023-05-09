using UserApiTestTaskVk.Domain.Entities;

namespace UserApiTestTaskVk.Application.Common.Interfaces;

/// <summary>
/// Интерфейс содержащий начальные константные сущности из БД
/// </summary>
public interface IHaveConstEntities
{
	/// <summary>
	/// Группа пользователя "Админ"
	/// </summary>
	public UserGroup AdminUserGroup { get; }

	/// <summary>
	/// Группа пользователя "Пользователь"
	/// </summary>
	public UserGroup DefaultUserGroup { get; }

	/// <summary>
	/// Статус пользователя "Активный"
	/// </summary>
	public UserState ActiveUserState { get; }

	/// <summary>
	/// Статус пользователя "Удаленный"
	/// </summary>
	public UserState BlockedUserState { get; }
}
