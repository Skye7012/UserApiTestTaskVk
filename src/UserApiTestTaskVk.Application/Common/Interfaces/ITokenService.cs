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
	/// <param name="userAccount">Пользователь, для которого создается токен</param>
	/// <returns>Токен</returns>
	string CreateAccessToken(User userAccount);

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

	/// <summary>
	/// Провалидировать Refresh токен, и получить пользователя, 
	/// которому принадлежит этот токен
	/// </summary>
	/// <param name="refreshToken">Refresh токен</param>
	/// <param name="cancellationToken">Токен отмены</param>
	/// <returns>Пользователя, которому принадлежит этот токен</returns>
	public Task<User> ValidateRefreshTokenAndReceiveUserAccountAsync(
		string refreshToken,
		CancellationToken cancellationToken = default);
}
