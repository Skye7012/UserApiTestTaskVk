using UserApiTestTaskVk.Domain.Entities;

namespace UserApiTestTaskVk.UnitTests.Mocks;

/// <summary>
/// Тестовый пользователь с не захэшированным паролем
/// </summary>
public class TestUser : User
{
	/// <summary>
	/// Пароль
	/// </summary>
	public string Password { get; set; } = default!;
}
