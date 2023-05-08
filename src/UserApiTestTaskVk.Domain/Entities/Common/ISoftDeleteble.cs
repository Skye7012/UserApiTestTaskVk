namespace UserApiTestTaskVk.Domain.Entities.Common;

/// <summary>
/// Сущность, поддерживающая мягкое удаление
/// </summary>
public interface ISoftDeletable
{
	/// <summary>
	/// Дата удаления
	/// </summary>
	public DateTime? RevokedOn { get; set; }
}
