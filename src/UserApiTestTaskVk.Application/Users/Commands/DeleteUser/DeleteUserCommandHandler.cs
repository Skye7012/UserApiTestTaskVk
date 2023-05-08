using MediatR;
using Microsoft.EntityFrameworkCore;
using UserApiTestTaskVk.Application.Common.Exceptions;
using UserApiTestTaskVk.Application.Common.Interfaces;
using UserApiTestTaskVk.Domain.Entities;

namespace UserApiTestTaskVk.Application.Users.Commands.DeleteUser;

/// <summary>
/// Обработчик для <see cref="DeleteUserCommand"/>
/// </summary>
public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
{
	private readonly IApplicationDbContext _context;
	private readonly IAuthorizationService _authorizationService;

	/// <summary>
	/// Конструктор
	/// </summary>
	/// <param name="context">Контекст БД</param>
	/// <param name="authorizationService">Сервис пользовательских данных</param>
	public DeleteUserCommandHandler(
		IApplicationDbContext context,
		IAuthorizationService authorizationService)
	{
		_context = context;
		_authorizationService = authorizationService;
	}

	/// <inheritdoc/>
	public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
	{
		var user = await _context.Users
			.Include(x => x.RefreshTokens)
			.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
			?? throw new EntityNotFoundProblem<User>(request.Id);

		_authorizationService.CheckUserPermissionRule(user);

		_context.Users.Remove(user);
		await _context.SaveChangesAsync(cancellationToken);
	}
}
