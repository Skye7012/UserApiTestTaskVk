using System;
using NSubstitute;
using UserApiTestTaskVk.Application.Common.Exceptions;
using UserApiTestTaskVk.Application.Common.Interfaces;
using UserApiTestTaskVk.Domain.Entities;
using UserApiTestTaskVk.UnitTests.Common.Interfaces;

namespace UserApiTestTaskVk.UnitTests.Mocks;

/// <summary>
/// Сервис авторизации для тестов
/// </summary>
public class AuthorizationServiceSubstitute : IAuthorizationService, ISubstitute<IAuthorizationService>
{
	private readonly User _adminUser;

	/// <summary>
	/// Конструктор
	/// </summary>
	/// <param name="adminUser">пользователь-администратор</param>
	public AuthorizationServiceSubstitute(User adminUser)
		=> _adminUser = adminUser;

	/// <inheritdoc/>
	public IAuthorizationService Create()
		=> Substitute.ForPartsOf<AuthorizationServiceSubstitute>(_adminUser);

	/// <inheritdoc/>
	public virtual bool IsAuthenticated()
		=> true;

	/// <inheritdoc/>
	public virtual Guid GetUserId()
		=> _adminUser.Id;

	/// <inheritdoc/>
	public virtual bool IsAdmin()
		=> true;

	/// <inheritdoc/>
	public virtual void CheckUserPermissionRule(User user)
	{
		if (user.Id != _adminUser.Id)
			throw new ForbiddenProblem();
	}
}
