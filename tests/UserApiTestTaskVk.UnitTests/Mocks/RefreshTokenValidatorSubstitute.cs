using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using UserApiTestTaskVk.Application.Common.Interfaces;
using UserApiTestTaskVk.Domain.Entities;
using UserApiTestTaskVk.UnitTests.Common.Interfaces;

namespace UserApiTestTaskVk.UnitTests.Mocks;

/// <summary>
/// Валидатор Refresh токенов для тестов
/// </summary>
public class RefreshTokenValidatorSubstitute : IRefreshTokenValidator, ISubstitute<IRefreshTokenValidator>
{
	private readonly User _adminUser;

	/// <summary>
	/// Конструктор
	/// </summary>
	/// <param name="adminUser">Пользователь-администратор</param>
	public RefreshTokenValidatorSubstitute(User adminUser)
		=> _adminUser = adminUser;

	/// <inheritdoc/>
	public IRefreshTokenValidator Create()
		=> Substitute.ForPartsOf<RefreshTokenValidatorSubstitute>(_adminUser);

	/// <inheritdoc/>
	public virtual async Task<User> ValidateAndReceiveUserAsync(
		IApplicationDbContext context,
		string refreshToken,
		CancellationToken cancellationToken = default)
			=> await context.Users.FirstAsync(x => x.Id == _adminUser.Id, cancellationToken);
}
