using System.Security.Claims;
using UserApiTestTaskVk.Application.Common.Exceptions;
using UserApiTestTaskVk.Application.Common.Static;

namespace UserApiTestTaskVk.Infrastructure.Extensions;

/// <summary>
/// Расширения для <see cref="ClaimsPrincipal"/>
/// </summary>
public static class ClaimsPrincipalExtensions
{
	/// <summary>
	/// Аутентифицировался ли пользователь
	/// </summary>
	/// <param name="source">Информация о пользователе</param>
	/// <returns>Аутентифицировался ли пользователь</returns>
	public static bool IsAuthenticated(this ClaimsPrincipal? source)
		=> source?.FindFirstValue(CustomClaims.UserIdСlaimName) != null
			&& source?.FindFirstValue(CustomClaims.IsAdminClaimName) != null;

	/// <summary>
	/// Получить идентификатор пользователя
	/// </summary>
	/// <param name="source">Информация о пользователе</param>
	/// <returns>Идентификатор пользователя</returns>
	public static Guid GetUserId(this ClaimsPrincipal? source)
	{
		var isParsed = Guid.TryParse(
				source?.FindFirstValue(CustomClaims.UserIdСlaimName),
				out var res);

		return !isParsed
			? throw new UnauthorizedProblem($"Невалидный клейм '{CustomClaims.UserIdСlaimName}' " +
				$"в токене")
			: res;
	}

	/// <summary>
	/// Проверить является ли пользователь администратором
	/// </summary>
	/// <param name="source">Информация о пользователе</param>
	/// <returns>Является ли пользователь администратором</returns>
	public static bool GetIsAdmin(this ClaimsPrincipal? source)
	{
		var isParsed = bool.TryParse(
				source?.FindFirstValue(CustomClaims.IsAdminClaimName),
				out var res);

		return !isParsed
			? throw new UnauthorizedProblem($"Невалидный клейм '{CustomClaims.IsAdminClaimName}' " +
				$"в токене")
			: res;
	}
}
