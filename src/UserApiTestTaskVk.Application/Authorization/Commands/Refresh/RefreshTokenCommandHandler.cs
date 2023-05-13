using MediatR;
using UserApiTestTaskVk.Application.Common.Interfaces;
using UserApiTestTaskVk.Contracts.Requests.Authorization.Refresh;
using UserApiTestTaskVk.Domain.Entities;

namespace UserApiTestTaskVk.Application.Authorization.Commands.Refresh;

/// <summary>
/// Обработчик для <see cref="RefreshTokenCommand"/>
/// </summary>
public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenResponse>
{
	private readonly IApplicationDbContext _context;
	private readonly ITokenService _tokenService;
	private readonly IRefreshTokenValidator _refreshTokenValidator;

	/// <summary>
	/// Конструктор
	/// </summary>
	/// <param name="context">Контекст БД</param>
	/// <param name="tokenService">Сервис JWT токенов</param>
	/// <param name="refreshTokenValidator">Валидатор Refresh токенов</param>
	public RefreshTokenCommandHandler(
		IApplicationDbContext context,
		ITokenService tokenService,
		IRefreshTokenValidator refreshTokenValidator)
	{
		_context = context;
		_tokenService = tokenService;
		_refreshTokenValidator = refreshTokenValidator;
	}

	/// <inheritdoc/>
	public async Task<RefreshTokenResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
	{
		var user = await _refreshTokenValidator.ValidateAndReceiveUserAsync(
			_context,
			request.RefreshToken,
			cancellationToken);

		string refreshToken = _tokenService.CreateRefreshToken();
		await _context.RefreshTokens
			.AddAsync(new RefreshToken(refreshToken, user), cancellationToken);

		await _context.SaveChangesAsync(cancellationToken);

		return new RefreshTokenResponse()
		{
			AccessToken = _tokenService.CreateAccessToken(user),
			RefreshToken = refreshToken,
		};
	}
}
