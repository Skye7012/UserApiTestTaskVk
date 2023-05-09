using UserApiTestTaskVk.Contracts.Common.Enums;
using UserApiTestTaskVk.Domain.Entities;

namespace UserApiTestTaskVk.Infrastructure.InitExecutors;

/// <summary>
/// Начальные константные сущности
/// </summary>
public class ConstEntities
{
	/// <summary>
	/// Группа пользователя "Админ"
	/// </summary>
	public static UserGroup AdminUserGroup => new()
	{
		Id = new Guid("fbc4e285-0375-4e05-af2b-0f9fe061c886"),
		Code = UserGroupCodes.Admin,
		Description = "Администратор",
		CreatedDate = Date,
	};

	/// <summary>
	/// Группа пользователя "Пользователь"
	/// </summary>
	public static UserGroup DefaultUserGroup => new()
	{
		Id = new Guid("8e76c7ec-4df1-4546-b39d-870a2f0cc126"),
		Code = UserGroupCodes.User,
		Description = "Пользователь",
		CreatedDate = Date,
	};

	/// <summary>
	/// Статус пользователя "Активный"
	/// </summary>
	public static UserState ActiveUserState => new()
	{
		Id = new Guid("16643212-60d8-416c-b9fd-41ef7ed2721b"),
		Code = UserStateCodes.Active,
		Description = "Активный",
		CreatedDate = Date,
	};

	/// <summary>
	/// Статус пользователя "Удаленный"
	/// </summary>
	public static UserState BlockedUserState => new()
	{
		Id = new Guid("da8b202c-21ef-4b44-b358-83cb1a5c9ca3"),
		Code = UserStateCodes.Blocked,
		Description = "Удаленный",
		CreatedDate = Date,
	};

	/// <summary>
	/// Дата создания начальных сущностей
	/// </summary>
	public static readonly DateTime Date
		= DateTime.SpecifyKind(
			new DateTime(2020, 01, 01),
			DateTimeKind.Utc);
}
