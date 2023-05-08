using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserApiTestTaskVk.Domain.Entities;
using UserApiTestTaskVk.Infrastructure.Extensions;
using UserApiTestTaskVk.Infrastructure.Persistence.Common;

namespace UserApiTestTaskVk.Infrastructure.Persistence.Configurations;

/// <summary>
/// Конфигурация для <see cref="RefreshToken"/>
/// </summary>
public class RefreshTokenConfiguration : EntityBaseConfiguration<RefreshToken>
{
	/// <inheritdoc/>
	public override void ConfigureChild(EntityTypeBuilder<RefreshToken> builder)
	{
		builder.HasComment("Refresh токен");

		builder.Property(e => e.Token);
		builder.Property(e => e.RevokedOn);

		builder.HasIndex(e => e.Token)
			.IsUnique();

		builder.HasOne(e => e.User)
			.WithMany(e => e.RefreshTokens)
			.HasForeignKey(e => e.UserId)
			.HasPrincipalKey(e => e.Id)
			.OnDelete(DeleteBehavior.ClientCascade)
			.HasField(User.RefreshTokensFieldName);
	}
}
