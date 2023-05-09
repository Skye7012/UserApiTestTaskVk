using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserApiTestTaskVk.Domain.Entities;
using UserApiTestTaskVk.Infrastructure.InitExecutors;
using UserApiTestTaskVk.Infrastructure.Persistence.Common;

namespace UserApiTestTaskVk.Infrastructure.Persistence.Configurations;

/// <summary>
/// Конфигурация для <see cref="UserState"/>
/// </summary>
public class UserStateConfiguration : EntityBaseConfiguration<UserState>
{
	/// <inheritdoc/>
	public override void ConfigureChild(EntityTypeBuilder<UserState> builder)
	{
		builder.HasComment("Статус пользователя");

		builder.Property(e => e.Code);
		builder.Property(e => e.Description);

		builder.HasIndex(e => e.Code)
			.IsUnique();

		builder.HasMany(e => e.Users)
			.WithOne(e => e.UserState)
			.HasForeignKey(e => e.UserStateId)
			.HasPrincipalKey(e => e.Id);

		builder.HasData(ConstEntities.ActiveUserState, ConstEntities.BlockedUserState);
	}
}
