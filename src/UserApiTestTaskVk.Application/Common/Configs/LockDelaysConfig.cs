namespace UserApiTestTaskVk.Application.Common.Configs;

/// <summary>
/// Конфигурация для задержек локов
/// </summary>
public class LockDelaysConfig
{
	/// <summary>
	/// Наименование секции в appSettings
	/// </summary>
	public const string ConfigSectionName = "LockDelaysConfig";

	/// <summary>
	/// Задержка для лока Пользователя в секундах
	/// </summary>
	public int UserLockDelay { get; set; } = 5;
}
