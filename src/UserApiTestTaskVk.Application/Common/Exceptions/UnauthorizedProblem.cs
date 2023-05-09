using System.Net;
using UserApiTestTaskVk.Domain.Exceptions;

namespace UserApiTestTaskVk.Application.Common.Exceptions;

/// <summary>
/// Не удается произвести аутентификацию
/// </summary>
public class UnauthorizedProblem : ApplicationProblem
{
	/// <summary>
	/// Конструктор
	/// </summary>
	/// <param name="message">Сообщение</param>
	public UnauthorizedProblem(string? message = null)
		: base(message ?? "Не удается произвести аутентификацию")
	{ }

	/// <inheritdoc/>
	public override HttpStatusCode StatusCode => HttpStatusCode.Unauthorized;
}
