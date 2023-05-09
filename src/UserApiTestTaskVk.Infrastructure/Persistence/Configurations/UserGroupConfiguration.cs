using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserApiTestTaskVk.Domain.Entities;
using UserApiTestTaskVk.Infrastructure.InitExecutors;
using UserApiTestTaskVk.Infrastructure.Persistence.Common;

namespace UserApiTestTaskVk.Infrastructure.Persistence.Configurations;

/// <summary>
/// Конфигурация для <see cref="UserGroup"/>
/// </summary>
public class UserGroupConfiguration : EntityBaseConfiguration<UserGroup>
{
	/// <inheritdoc/>
	public override void ConfigureChild(EntityTypeBuilder<UserGroup> builder)
	{
		builder.HasComment("Группа пользователя");

		builder.Property(e => e.Code);
		builder.Property(e => e.Description);

		builder.HasIndex(e => e.Code)
			.IsUnique();

		builder.HasMany(e => e.Users)
			.WithOne(e => e.UserGroup)
			.HasForeignKey(e => e.UserGroupId)
			.HasPrincipalKey(e => e.Id);

		builder.HasData(ConstEntities.AdminUserGroup, ConstEntities.DefaultUserGroup);
	}
}
