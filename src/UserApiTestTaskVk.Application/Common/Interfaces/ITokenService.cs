using System.Security.Claims;
using UserApiTestTaskVk.Domain.Entities;

namespace UserApiTestTaskVk.Application.Common.Interfaces;

/// <summary>
/// Сервис JWT токенов
/// </summary>
public interface ITokenService
{
	/// <summary>
	/// Создать Access токен
	/// </summary>
	/// <param name="user">Пользователь, для которого создается токен</param>
	/// <returns>Токен</returns>
	string CreateAccessToken(User user);

	/// <summary>
	/// Создать Reresh токен
	/// </summary>
	/// <returns>Токен</returns>
	string CreateRefreshToken();

	/// <summary>
	/// Создать JWT токен
	/// </summary>
	/// <param name="expires">Время окончания действия токена</param>
	/// <param name="claims">Клеймы</param>
	/// <returns>Токен</returns>
	public string CreateToken(DateTime expires, IEnumerable<Claim>? claims = null);
}
