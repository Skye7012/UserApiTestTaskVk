namespace UserApiTestTaskVk.Domain.Entities.Common;

/// <summary>
/// Базовая сущность
/// </summary>
public abstract class EntityBase
{
	/// <summary>
	/// Идентификатор
	/// </summary>
	public Guid Id { get; set; }

	/// <summary>
	/// Дата создания сущности
	/// </summary>
	public DateTime CreatedDate { get; set; }
}
