using UserApiTestTaskVk.Domain.Entities;

namespace UserApiTestTaskVk.Application.Common.Interfaces;

/// <summary>
/// Сервис авторизации
/// </summary>
public interface IAuthorizationService
{
	/// <summary>
	/// Аутентифицировался ли пользователь
	/// </summary>
	/// <returns>Аутентифицировался ли пользователь</returns>
	bool IsAuthenticated();

	/// <summary>
	/// Получить идентификатор пользователя по клейму в токене
	/// </summary>
	/// <returns>Идентификатор пользователя</returns>
	public Guid GetUserId();

	/// <summary>
	/// Является ли авторизованный пользователь администратором
	/// </summary>
	/// <returns>Является ли авторизованный пользователь администратором</returns>
	bool IsAdmin();

	/// <summary>
	/// Проверить, что переданный пользователь соответствует аутентифицированному,
	///  и что переданный пользователь активен<br/>
	/// Либо аутентифицированный пользователь является администратором
	/// </summary>
	/// <param name="user">Пользователь</param>
	void CheckUserPermissionRule(User user);
}
