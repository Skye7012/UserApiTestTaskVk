namespace UserApiTestTaskVk.Application.Common.Static;

/// <summary>
/// Кастомные клэймы для JWT токена
/// </summary>
public class CustomClaims
{
	/// <summary>
	/// Наименование клейма "Идентификатор пользователя"
	/// </summary>
	public const string UserIdСlaimName = "userId";

	/// <summary>
	/// Наименование клейма "Является ли пользователь администратором"
	/// </summary>
	public const string IsAdminClaimName = "isAdmin";
}
