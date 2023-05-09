using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserApiTestTaskVk.Application.Authorization.Commands.PutUserLogin;
using UserApiTestTaskVk.Application.Authorization.Commands.PutUserPassword;
using UserApiTestTaskVk.Application.Authorization.Commands.Refresh;
using UserApiTestTaskVk.Application.Authorization.Commands.SignIn;
using UserApiTestTaskVk.Application.Authorization.Commands.SignOut;
using UserApiTestTaskVk.Application.Authorization.Commands.SignUp;
using UserApiTestTaskVk.Contracts.Requests.Authorization.PutUserLogin;
using UserApiTestTaskVk.Contracts.Requests.Authorization.PutUserPassword;
using UserApiTestTaskVk.Contracts.Requests.Authorization.Refresh;
using UserApiTestTaskVk.Contracts.Requests.Authorization.SignIn;
using UserApiTestTaskVk.Contracts.Requests.Authorization.SignUp;

namespace UserApiTestTaskVk.Api.Controllers;

/// <summary>
/// Контроллер авторизации
/// </summary>
[ApiController]
[Route("[controller]")]
public class AuthorizationController : ControllerBase
{
	private readonly IMediator _mediator;

	/// <summary>
	/// Конструктор
	/// </summary>
	/// <param name="mediator">Медиатор</param>
	public AuthorizationController(IMediator mediator)
		=> _mediator = mediator;

	/// <summary>
	/// Зарегистрироваться
	/// </summary>
	/// <param name="request">Запрос</param>
	/// <param name="cancellationToken">Токен отмены</param>
	/// <returns>Идентификатор созданного пользователя</returns>
	[HttpPost("SignUp")]
	[ProducesResponseType(StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<ActionResult<SignUpResponse>> SignUpAsync(
		SignUpRequest request,
		CancellationToken cancellationToken)
	{
		var response = await _mediator.Send(
			new SignUpCommand
			{
				Login = request.Login,
				Password = request.Password,
			},
			cancellationToken);

		return CreatedAtAction(
			nameof(UserController.GetByIdAsync),
			"User",
			new { id = response.UserId },
			response);
	}

	/// <summary>
	/// Авторизоваться
	/// </summary>
	/// <param name="request">Запрос</param>
	/// <param name="cancellationToken">Токен отмены</param>
	/// <returns>Токены авторизации</returns>
	[HttpPost("SignIn")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<SignInResponse> SignInAsync(
		SignInRequest request,
		CancellationToken cancellationToken)
		=> await _mediator.Send(
			new SignInCommand
			{
				Login = request.Login,
				Password = request.Password,
			},
			cancellationToken);

	/// <summary>
	/// Обновить токены
	/// </summary>
	/// <param name="request">Запрос</param>
	/// <param name="cancellationToken">Токен отмены</param>
	/// <returns>Обновленные токены</returns>
	[HttpPost("Refresh")]
	[Authorize]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<RefreshTokenResponse> RefreshAsync(
		RefreshTokenRequest request,
		CancellationToken cancellationToken)
		=> await _mediator.Send(
			new RefreshTokenCommand
			{
				RefreshToken = request.RefreshToken,
			},
			cancellationToken);

	/// <summary>
	/// Выйти (завершить все сеансы)
	/// </summary>
	/// <param name="cancellationToken">Токен отмены</param>
	/// <returns>Обновленные токены</returns>
	[HttpDelete("SignOut")]
	[Authorize]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task SignOutAsync(CancellationToken cancellationToken)
		=> await _mediator.Send(
			new SignOutCommand(),
			cancellationToken);

	/// <summary>
	/// Изменить пароль пользователя
	/// </summary>
	/// <remarks>Доступно администратору, либо лично пользователю, если он активен</remarks>
	/// <param name="id">Идентификатор пользователя</param>
	/// <param name="request">Запрос</param>
	/// <param name="cancellationToken">Токен отмены</param>
	[HttpPut("ChangePassword/{id}")]
	[Authorize]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<PutUserPasswordResponse> PutPasswordAsync(
		[FromRoute] Guid id,
		PutUserPasswordRequest request,
		CancellationToken cancellationToken)
			=> await _mediator.Send(
				new PutUserPasswordCommand(id)
				{
					OldPassword = request.OldPassword,
					NewPassword = request.NewPassword,
				},
				cancellationToken);

	/// <summary>
	/// Изменить логин пользователя
	/// </summary>
	/// <remarks>Доступно администратору, либо лично пользователю, если он активен</remarks>
	/// <param name="id">Идентификатор пользователя</param>
	/// <param name="request">Запрос</param>
	/// <param name="cancellationToken">Токен отмены</param>
	[HttpPut("ChangeLogin/{id}")]
	[Authorize]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<PutUserLoginResponse> PutLoginAsync(
		[FromRoute] Guid id,
		PutUserLoginRequest request,
		CancellationToken cancellationToken)
			=> await _mediator.Send(
				new PutUserLoginCommand(id)
				{
					NewLogin = request.NewLogin,
				},
				cancellationToken);
}
