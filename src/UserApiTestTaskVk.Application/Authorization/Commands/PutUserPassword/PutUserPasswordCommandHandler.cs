using System.Text.RegularExpressions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserApiTestTaskVk.Application.Common.Exceptions;
using UserApiTestTaskVk.Application.Common.Interfaces;
using UserApiTestTaskVk.Contracts.Requests.Authorization.PutUserPassword;
using UserApiTestTaskVk.Domain.Entities;
using UserApiTestTaskVk.Domain.Exceptions;

namespace UserApiTestTaskVk.Application.Authorization.Commands.PutUserPassword;

/// <summary>
/// Обработчик для <see cref="PutUserPasswordCommand"/>
/// </summary>
public class PutUserPasswordCommandHandler : IRequestHandler<PutUserPasswordCommand, PutUserPasswordResponse>
{
	private readonly IApplicationDbContext _context;
	private readonly IAuthorizationService _authorizationService;
	private readonly IPasswordService _passwordService;
	private readonly ITokenService _tokenService;

	/// <summary>
	/// Конструктор
	/// </summary>
	/// <param name="context">Контекст БД</param>
	/// <param name="authorizationService">Сервис авторизации</param>
	/// <param name="passwordService">Сервис паролей</param>
	/// <param name="tokenService">Сервис JWT токенов</param>
	public PutUserPasswordCommandHandler(
		IApplicationDbContext context,
		IAuthorizationService authorizationService,
		IPasswordService passwordService,
		ITokenService tokenService)
	{
		_context = context;
		_authorizationService = authorizationService;
		_passwordService = passwordService;
		_tokenService = tokenService;
	}

	/// <inheritdoc/>
	public async Task<PutUserPasswordResponse> Handle(PutUserPasswordCommand request, CancellationToken cancellationToken)
	{
		var user = await _context.Users
			.Include(x => x.RefreshTokens)
			.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
			?? throw new EntityNotFoundProblem<User>(request.Id);

		_authorizationService.CheckUserPermissionRule(user);

		if (!_passwordService.VerifyPasswordHash(request.OldPassword, user.PasswordHash, user.PasswordSalt))
			throw new ValidationProblem("Введен неверный текущий пароль пользователя");

		if (!Regex.IsMatch(request.NewPassword, @"^[a-zA-Z0-9]+$"))
			throw new ValidationProblem("Для пароля запрещены все символы кроме латинских букв и цифр");

		_passwordService.CreatePasswordHash(request.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);

		user.PasswordHash = passwordHash;
		user.PasswordSalt = passwordSalt;

		user.RevokeAllRefreshTokens();
		string refreshToken = _tokenService.CreateRefreshToken();
		user.AddRefreshToken(new RefreshToken(refreshToken, user));

		await _context.SaveChangesAsync(cancellationToken);

		return new PutUserPasswordResponse
		{
			AccessToken = _tokenService.CreateAccessToken(user),
			RefreshToken = refreshToken,
		};
	}
}
