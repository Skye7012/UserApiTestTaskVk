using MediatR;
using Microsoft.EntityFrameworkCore;
using UserApiTestTaskVk.Application.Common.Exceptions;
using UserApiTestTaskVk.Application.Common.Interfaces;
using UserApiTestTaskVk.Contracts.Requests.Users.GetUser;
using UserApiTestTaskVk.Domain.Entities;

namespace UserApiTestTaskVk.Application.Users.Queries.GetUserById;

/// <summary>
/// Обработчик для of <see cref="GetUserByIdQuery"/>
/// </summary>
public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, GetUserResponse>
{
	private readonly IApplicationDbContext _context;
	private readonly IAuthorizationService _authorizationService;

	/// <summary>
	/// Конструктор
	/// </summary>
	/// <param name="context">Контекст БД</param>
	/// <param name="authorizationService">Сервис пользовательских данных</param>
	public GetUserByIdQueryHandler(IApplicationDbContext context, IAuthorizationService authorizationService)
	{
		_context = context;
		_authorizationService = authorizationService;
	}

	/// <inheritdoc/>
	public async Task<GetUserResponse> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
	{
		var user = await _context.Users
			.Include(x => x.UserGroup)
			.Include(x => x.UserState)
			.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
			?? throw new EntityNotFoundProblem<User>(request.Id);

		_authorizationService.CheckUserPermissionRule(user);

		return new GetUserResponse()
		{
			Id = user.Id,
			Login = user.Login,
			Group = new GetUserGroupResponse
			{
				Id = user.UserGroup!.Id,
				Code = user.UserGroup!.Code,
				Description = user.UserGroup!.Description,
			},
			State = new GetUserStateResponse
			{
				Id = user.UserState!.Id,
				Code = user.UserState!.Code,
				Description = user.UserState!.Description,
			},
		};
	}
}
