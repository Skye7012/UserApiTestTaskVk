using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserApiTestTaskVk.Api.AuthorizationAttributes;
using UserApiTestTaskVk.Application.Users.Commands.DeleteUser;
using UserApiTestTaskVk.Application.Users.Queries.GetUserById;
using UserApiTestTaskVk.Application.Users.Queries.GetUsers;
using UserApiTestTaskVk.Contracts.Requests.Users.GetUser;
using UserApiTestTaskVk.Contracts.Requests.Users.GetUsers;

namespace UserApiTestTaskVk.Api.Controllers;

/// <summary>
/// Контроллер для Пользователей
/// </summary>
[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
	private readonly IMediator _mediator;

	/// <summary>
	/// Конструктор
	/// </summary>
	/// <param name="mediator">Медиатор</param>
	public UserController(IMediator mediator)
		=> _mediator = mediator;

	/// <summary>
	/// Получить данные о пользователях
	/// </summary>
	/// <remarks>Доступно только администратору</remarks>
	/// <param name="request">Запрос</param>
	/// <param name="cancellationToken">Токен отмены</param>
	/// <returns>Данные о пользователях</returns>
	[HttpGet]
	[Authorize]
	[AdminAuthorization]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<GetUsersResponse> GetAsync(
		[FromQuery] GetUsersRequest request,
		CancellationToken cancellationToken = default)
			=> await _mediator.Send(new GetUsersQuery()
			{
				IsAscending = request.IsAscending,
				Limit = request.Limit,
				OrderBy = request.OrderBy,
				Page = request.Page,
			},
			cancellationToken);

	/// <summary>
	/// Получить данные о пользователе
	/// </summary>
	/// <remarks>Доступно администратору, либо лично пользователю, если он активен</remarks>
	/// <param name="id">Идентификатор пользователя</param>
	/// <param name="cancellationToken">Токен отмены</param>
	/// <returns>Данные о пользователе</returns>
	[HttpGet("{id}")]
	[Authorize]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<GetUserResponse> GetByIdAsync(
		[FromRoute] Guid id,
		CancellationToken cancellationToken = default)
			=> await _mediator.Send(new GetUserByIdQuery(id), cancellationToken);

	/// <summary>
	/// Удалить пользователя
	/// </summary>
	/// <remarks>Доступно администратору, либо лично пользователю, если он активен</remarks>
	/// <param name="id">Идентификатор пользователя</param>
	/// <param name="cancellationToken">Токен отмены</param>
	[HttpDelete("{id}")]
	[Authorize]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task DeleteAsync(
		[FromRoute] Guid id,
		CancellationToken cancellationToken = default)
			=> await _mediator.Send(
				new DeleteUserCommand(id),
				cancellationToken);
}
