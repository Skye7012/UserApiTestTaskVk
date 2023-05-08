using MediatR;
using Microsoft.EntityFrameworkCore;
using UserApiTestTaskVk.Application.Common.Extensions;
using UserApiTestTaskVk.Application.Common.Interfaces;
using UserApiTestTaskVk.Contracts.Requests.Users.GetUser;
using UserApiTestTaskVk.Contracts.Requests.Users.GetUsers;

namespace UserApiTestTaskVk.Application.Users.Queries.GetUsers;

/// <summary>
/// Обработчик для of <see cref="GetUsersQuery"/>
/// </summary>
public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, GetUsersResponse>
{
	private readonly IApplicationDbContext _context;

	/// <summary>
	/// Конструктор
	/// </summary>
	/// <param name="context">Контекст БД</param>
	public GetUsersQueryHandler(IApplicationDbContext context)
		=> _context = context;

	/// <inheritdoc/>
	public async Task<GetUsersResponse> Handle(GetUsersQuery request, CancellationToken cancellationToken)
	{
		var query = _context.Users
			.Select(x => new GetUserResponse
			{
				Id = x.Id,
				Login = x.Login,
				Group = new GetUserGroupResponse
				{
					Id = x.UserGroup!.Id,
					Code = x.UserGroup!.Code,
					Description = x.UserGroup!.Description,
				},
				State = new GetUserStateResponse
				{
					Id = x.UserState!.Id,
					Code = x.UserState!.Code,
					Description = x.UserState!.Description,
				},
			});

		return new GetUsersResponse()
		{
			TotalCount = await query.CountAsync(cancellationToken),
			Items = await query
				.Sort(request)
				.ToListAsync(cancellationToken),
		};
	}
}
