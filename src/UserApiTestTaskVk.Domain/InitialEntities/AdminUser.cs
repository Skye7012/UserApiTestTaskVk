using UserApiTestTaskVk.Domain.Entities;

namespace UserApiTestTaskVk.Domain.InitialEntities;

/// <summary>
/// Администратор
/// </summary>
public class AdminUser : User
{
	/// <summary>
	/// Логин пользователя-администратора, который создается автоматически
	/// </summary>
	public const string AdminLogin = "Admin";

	/// <summary>
	/// Конструктор
	/// </summary>
	/// <param name="passwordHash">Хэш пароля</param>
	/// <param name="passwordSalt">Соль пароля</param>
	public AdminUser(
		byte[] passwordHash,
		byte[] passwordSalt)
		: base(AdminLogin, passwordHash, passwordSalt)
	{
		UserGroup = ConstEntities.AdminUserGroup;
		UserState = ConstEntities.ActiveUserState;
	}

	/// <summary>
	/// Конструктор
	/// </summary>
	public AdminUser() { }
}
