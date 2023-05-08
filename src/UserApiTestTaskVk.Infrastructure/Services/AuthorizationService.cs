using Microsoft.AspNetCore.Http;
using UserApiTestTaskVk.Application.Common.Exceptions;
using UserApiTestTaskVk.Application.Common.Interfaces;
using UserApiTestTaskVk.Contracts.Common.Enums;
using UserApiTestTaskVk.Domain.Entities;
using UserApiTestTaskVk.Domain.Exceptions;
using UserApiTestTaskVk.Infrastructure.Extensions;

namespace UserApiTestTaskVk.Infrastructure.Services;

/// <summary>
/// Сервис авторизации
/// </summary>
public class AuthorizationService : IAuthorizationService
{
	private readonly IHttpContextAccessor _httpContextAccessor;

	/// <summary>
	/// Конструктор
	/// </summary>
	/// <param name="httpContextAccessor">HTTP контекст запроса</param>
	public AuthorizationService(IHttpContextAccessor httpContextAccessor)
		=> _httpContextAccessor = httpContextAccessor;

	/// <inheritdoc/>
	public bool IsAuthenticated()
		=> _httpContextAccessor.HttpContext?.User.IsAuthenticated() == true;

	/// <inheritdoc/>
	public Guid GetUserId()
		=> _httpContextAccessor.HttpContext!.User.GetUserId();

	/// <inheritdoc/>
	public bool IsAdmin()
		=> _httpContextAccessor.HttpContext!.User.GetIsAdmin();

	/// <inheritdoc/>
	public void CheckUserPermissionRule(User user)
	{
		if (IsAdmin())
			return;

		if (user.UserState is null)
			throw new NotIncludedProblem(nameof(User.UserState));

		if (user.Id != GetUserId() || user.UserState!.Code == UserStateCodes.Blocked)
			throw new ForbiddenProblem("Данное действие доступно администратору, " +
				"либо лично пользователю, если он активен");
	}
}
