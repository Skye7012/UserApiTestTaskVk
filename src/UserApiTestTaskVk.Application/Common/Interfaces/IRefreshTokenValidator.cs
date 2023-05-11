using UserApiTestTaskVk.Domain.Entities;

namespace UserApiTestTaskVk.Application.Common.Interfaces;

/// <summary>
/// Валидатор Refresh токенов
/// </summary>
public interface IRefreshTokenValidator
{
	/// <summary>
	/// Провалидировать Refresh токен, и получить пользователя, 
	/// которому принадлежит этот токен
	/// </summary>
	/// <param name="context">Контекст БД</param>
	/// <param name="refreshToken">Refresh токен</param>
	/// <param name="cancellationToken">Токен отмены</param>
	/// <returns>Пользователя, которому принадлежит этот токен</returns>
	public Task<User> ValidateAndReceiveUserAsync(
		IApplicationDbContext context,
		string refreshToken,
		CancellationToken cancellationToken = default);
}
