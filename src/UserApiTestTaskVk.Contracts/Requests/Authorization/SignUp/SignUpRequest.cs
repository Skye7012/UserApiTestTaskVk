namespace UserApiTestTaskVk.Contracts.Requests.Authorization.SignUp;

/// <summary>
/// Запрос для регистрации пользователя
/// </summary>
public class SignUpRequest
{
	/// <summary>
	/// Логин
	/// </summary>
	public string Login { get; set; } = default!;

	/// <summary>
	/// Пароль
	/// </summary>
	public string Password { get; set; } = default!;
}
