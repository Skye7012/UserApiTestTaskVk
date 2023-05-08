using UserApiTestTaskVk.Domain.Entities.Common;

namespace UserApiTestTaskVk.Domain.Entities;

/// <summary>
/// Refresh токен
/// </summary>
public class RefreshToken : EntityBase, ISoftDeletable
{
	/// <summary>
	/// Конструктор
	/// </summary>
	/// <param name="token">Токен</param>
	/// <param name="user">Пользователь</param>
	public RefreshToken(
		string token,
		User user)
	{
		Token = token;
		User = user;
	}

	/// <summary>
	/// Конструктор
	/// </summary>
	public RefreshToken() { }

	/// <summary>
	/// Токен
	/// </summary>
	public string Token { get; set; } = default!;

	/// <inheritdoc/>
	public DateTime? RevokedOn { get; set; }

	/// <summary>
	/// Идентификатор Пользователя
	/// </summary>
	public Guid UserId { get; set; }

	#region navigation Properties

	/// <summary>
	/// Пользователь
	/// </summary>
	public User? User { get; set; }

	#endregion
}
