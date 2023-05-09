using MediatR;
using Microsoft.EntityFrameworkCore;
using UserApiTestTaskVk.Application.Common.Exceptions;
using UserApiTestTaskVk.Application.Common.Interfaces;
using UserApiTestTaskVk.Contracts.Requests.Authorization.PutUserLogin;
using UserApiTestTaskVk.Domain.Entities;
using UserApiTestTaskVk.Domain.Exceptions;

namespace UserApiTestTaskVk.Application.Authorization.Commands.PutUserLogin;

/// <summary>
/// Обработчик для <see cref="PutUserLoginCommand"/>
/// </summary>
public class PutUserLoginCommandHandler : IRequestHandler<PutUserLoginCommand, PutUserLoginResponse>
{
	private readonly IApplicationDbContext _context;
	private readonly IAuthorizationService _authorizationService;
	private readonly ITokenService _tokenService;

	/// <summary>
	/// Конструктор
	/// </summary>
	/// <param name="context">Контекст БД</param>
	/// <param name="authorizationService">Сервис авторизации</param>
	/// <param name="tokenService">Сервис JWT токенов</param>
	public PutUserLoginCommandHandler(
		IApplicationDbContext context,
		IAuthorizationService authorizationService,
		ITokenService tokenService)
	{
		_context = context;
		_authorizationService = authorizationService;
		_tokenService = tokenService;
	}

	/// <inheritdoc/>
	public async Task<PutUserLoginResponse> Handle(PutUserLoginCommand request, CancellationToken cancellationToken)
	{
		var user = await _context.Users
			.Include(x => x.RefreshTokens)
			.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
			?? throw new EntityNotFoundProblem<User>(request.Id);

		_authorizationService.CheckUserPermissionRule(user);

		var isNewLoginUnique = await _context.Users
			.AllAsync(x => x.Login != request.NewLogin, cancellationToken);

		if (!isNewLoginUnique)
			throw new ValidationProblem("Пользователь с таким логином уже существует, " +
				"новый логин должен быть уникальным");

		user.Login = request.NewLogin;

		user.RevokeAllRefreshTokens();
		string refreshToken = _tokenService.CreateRefreshToken();
		user.AddRefreshToken(new RefreshToken(refreshToken, user));

		await _context.SaveChangesAsync(cancellationToken);

		return new PutUserLoginResponse
		{
			AccessToken = _tokenService.CreateAccessToken(user),
			RefreshToken = refreshToken,
		};
	}
}
