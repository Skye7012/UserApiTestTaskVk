using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserApiTestTaskVk.Domain.Entities;
using UserApiTestTaskVk.Infrastructure.Extensions;
using UserApiTestTaskVk.Infrastructure.Persistence.Common;

namespace UserApiTestTaskVk.Infrastructure.Persistence.Configurations;

/// <summary>
/// Конфигурация для <see cref="User"/>
/// </summary>
public class UserConfiguration : EntityBaseConfiguration<User>
{
	/// <inheritdoc/>
	public override void ConfigureChild(EntityTypeBuilder<User> builder)
	{
		builder.HasComment("Пользователи");

		builder.Property(e => e.Login);
		builder.Property(e => e.PasswordHash);
		builder.Property(e => e.PasswordSalt);

		builder.HasIndex(e => e.Login)
			.IsUnique();

		builder.HasOne(e => e.UserGroup)
			.WithMany(e => e.Users)
			.HasForeignKey(e => e.UserGroupId)
			.HasPrincipalKey(e => e.Id);

		builder.HasOne(e => e.UserState)
			.WithMany(e => e.Users)
			.HasForeignKey(e => e.UserStateId)
			.HasPrincipalKey(e => e.Id);

		builder.HasMany(e => e.RefreshTokens)
			.WithOne(e => e.User)
			.HasForeignKey(e => e.UserId)
			.HasPrincipalKey(e => e.Id)
			.OnDelete(DeleteBehavior.ClientCascade)
			.HasField(User.RefreshTokensFieldName);
	}
}
