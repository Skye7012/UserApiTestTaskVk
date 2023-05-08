using MediatR;
using UserApiTestTaskVk.Contracts.Requests.Users.GetUsers;

namespace UserApiTestTaskVk.Application.Users.Queries.GetUsers;

/// <summary>
/// Запрос на получение пользователей
/// </summary>
public class GetUsersQuery : GetUsersRequest, IRequest<GetUsersResponse>
{
}
