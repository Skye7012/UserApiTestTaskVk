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

	/// <summary>
	/// Конструктор
	/// </summary>
	/// <param name="context">Контекст БД</param>
	/// <param name="tokenService">Сервис JWT токенов</param>
	public RefreshTokenCommandHandler(
		IApplicationDbContext context,
		ITokenService tokenService)
	{
		_context = context;
		_tokenService = tokenService;
	}

	/// <inheritdoc/>
	public async Task<RefreshTokenResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
	{
		var userAccount = await _tokenService.ValidateRefreshTokenAndReceiveUserAccountAsync(
			request.RefreshToken,
			cancellationToken);

		string refreshToken = _tokenService.CreateRefreshToken();
		await _context.RefreshTokens
			.AddAsync(new RefreshToken(refreshToken, userAccount), cancellationToken);
		await _context.SaveChangesAsync(cancellationToken);

		return new RefreshTokenResponse()
		{
			AccessToken = _tokenService.CreateAccessToken(userAccount),
			RefreshToken = refreshToken,
		};
	}
}
