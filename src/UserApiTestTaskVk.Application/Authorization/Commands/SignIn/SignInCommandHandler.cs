using MediatR;
using Microsoft.EntityFrameworkCore;
using UserApiTestTaskVk.Application.Common.Exceptions;
using UserApiTestTaskVk.Application.Common.Interfaces;
using UserApiTestTaskVk.Contracts.Common.Enums;
using UserApiTestTaskVk.Contracts.Requests.Authorization.SignIn;
using UserApiTestTaskVk.Domain.Entities;
using UserApiTestTaskVk.Domain.Exceptions;

namespace UserApiTestTaskVk.Application.Authorization.Commands.SignIn;

/// <summary>
/// Обработчик для <see cref="SignInCommand"/>
/// </summary>
public class SignInCommandHandler : IRequestHandler<SignInCommand, SignInResponse>
{
	private readonly IApplicationDbContext _context;
	private readonly ITokenService _tokenService;
	private readonly IPasswordService _passwordService;

	/// <summary>
	/// Конструктор
	/// </summary>
	/// <param name="context">Контекст БД</param>
	/// <param name="tokenService">Сервис JWT токенов</param>
	/// <param name="passwordService">Сервис паролей</param>
	public SignInCommandHandler(
		IApplicationDbContext context,
		ITokenService tokenService,
		IPasswordService passwordService)
	{
		_context = context;
		_tokenService = tokenService;
		_passwordService = passwordService;
	}

	/// <inheritdoc/>
	public async Task<SignInResponse> Handle(SignInCommand request, CancellationToken cancellationToken)
	{
		var user = await _context.Users
			.Include(x => x.UserState)
			.Include(x => x.RefreshTokens)
			.FirstOrDefaultAsync(x => x.Login == request.Login, cancellationToken)
			?? throw new UserNotFoundProblem(request.Login);

		if (user.UserState!.Code == UserStateCodes.Blocked)
			throw new ValidationProblem("Данные учетные данные были деактивированы");

		if (!_passwordService.VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
		{
			throw new ValidationProblem("Неправильный пароль");
		}

		string refreshToken = _tokenService.CreateRefreshToken();
		user.AddRefreshToken(new RefreshToken(refreshToken, user));

		await _context.SaveChangesAsync(cancellationToken);

		return new SignInResponse()
		{
			AccessToken = _tokenService.CreateAccessToken(user),
			RefreshToken = refreshToken,
		};
	}
}
