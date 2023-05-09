using System.Text.RegularExpressions;
using UserApiTestTaskVk.Domain.Entities.Common;
using UserApiTestTaskVk.Domain.Exceptions;

namespace UserApiTestTaskVk.Domain.Entities;

/// <summary>
/// Пользователь
/// </summary>
public class User : EntityBase
{
	/// <summary>
	/// Наименование backing field <see cref="_refreshTokens"/>
	/// </summary>
	public static readonly string RefreshTokensFieldName = nameof(_refreshTokens);

	/// <summary>
	/// Поле для <see cref="Login"/>
	/// </summary>
	private string _login = default!;

	/// <summary>
	/// Поле для <see cref="RefreshTokens"/>
	/// </summary>
	private readonly List<RefreshToken>? _refreshTokens;

	/// <summary>
	/// Конструктор
	/// </summary>
	/// <param name="login">Логин</param>
	/// <param name="passwordHash">Хэш пароля</param>
	/// <param name="passwordSalt">Соль пароля</param>
	/// <param name="userGroup">Группа пользователя</param>
	/// <param name="userState">Статус пользователя</param>
	public User(
		string login,
		byte[] passwordHash,
		byte[] passwordSalt,
		UserGroup userGroup,
		UserState userState)
	{
		Login = login;
		PasswordHash = passwordHash;
		PasswordSalt = passwordSalt;

		UserGroup = userGroup;
		UserState = userState;

		_refreshTokens = new List<RefreshToken>();
	}

	/// <summary>
	/// Конструктор
	/// </summary>
	public User() { }

	/// <summary>
	/// Логин
	/// </summary>
	public string Login
	{
		get => _login;
		set
		{
			if (string.IsNullOrWhiteSpace(value))
				throw new ValidationProblem($"Поле {nameof(Login)} не может быть пустым");

			if (!Regex.IsMatch(value, @"^[a-zA-Z0-9]+$"))
				throw new ValidationProblem("Для логина запрещены все символы кроме латинских букв и цифр");

			_login = value;
		}
	}

	/// <summary>
	/// Хэш пароля
	/// </summary>
	public byte[] PasswordHash { get; set; } = default!;

	/// <summary>
	/// Соль пароля
	/// </summary>
	public byte[] PasswordSalt { get; set; } = default!;

	/// <summary>
	/// Идентификатор Группы пользователя
	/// </summary>
	public Guid UserGroupId { get; private set; }

	/// <summary>
	/// Идентификатор Статуса пользователя
	/// </summary>
	public Guid UserStateId { get; private set; }

	/// <summary>
	/// Деактивировать все refresh токены
	/// </summary>
	public void RevokeAllRefreshTokens()
	{
		if (_refreshTokens == null)
			throw new NotIncludedProblem(nameof(_refreshTokens));

		_refreshTokens.Clear();
	}

	/// <summary>
	/// Добавить refresh токен
	/// </summary>
	public void AddRefreshToken(RefreshToken refreshToken)
	{
		if (_refreshTokens == null)
			throw new NotIncludedProblem(nameof(_refreshTokens));


		var activeTokens = _refreshTokens.Where(r => r.RevokedOn == null);

		if (activeTokens.Count() >= 5)
			_refreshTokens.Remove(
				activeTokens.First(x => x.CreatedDate == activeTokens.Max(x => x.CreatedDate)));

		_refreshTokens.Add(refreshToken);
	}

	#region navigation Properties

	/// <summary>
	/// Группа пользователя
	/// </summary>
	public UserGroup? UserGroup { get; set; }

	/// <summary>
	/// Статус пользователя
	/// </summary>
	public UserState? UserState { get; set; }

	/// <summary>
	/// Refresh токены
	/// </summary>
	public IReadOnlyList<RefreshToken>? RefreshTokens => _refreshTokens;

	#endregion
}
