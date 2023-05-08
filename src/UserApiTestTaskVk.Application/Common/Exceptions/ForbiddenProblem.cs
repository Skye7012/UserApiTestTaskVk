using System.Net;
using UserApiTestTaskVk.Domain.Exceptions;

namespace UserApiTestTaskVk.Application.Common.Exceptions;

/// <summary>
/// У пользователя нет прав на выполнение данной операции 
/// </summary>
public class ForbiddenProblem : ApplicationProblem
{
	/// <summary>
	/// Конструктор
	/// </summary>
	/// <param name="message">Сообщение</param>
	public ForbiddenProblem(string? message = null)
		: base(message ?? "У пользователя нет прав на выполнение данного действия")
	{ }

	/// <inheritdoc/>
	public override HttpStatusCode StatusCode => HttpStatusCode.Forbidden;
}
