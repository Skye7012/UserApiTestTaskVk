using System.Data;
using System.Text.RegularExpressions;
using Medallion.Threading.Postgres;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserApiTestTaskVk.Application.Common.Interfaces;
using UserApiTestTaskVk.Contracts.Requests.Authorization.SignUp;
using UserApiTestTaskVk.Domain.Entities;
using UserApiTestTaskVk.Domain.Exceptions;

namespace UserApiTestTaskVk.Application.Authorization.Commands.SignUp;

/// <summary>
/// Обработчик для <see cref="SignUpCommand"/>
/// </summary>
public class SignUpCommandHandler : IRequestHandler<SignUpCommand, SignUpResponse>
{
	private readonly IApplicationDbContext _context;
	private readonly IPasswordService _passwordService;
	private readonly IDbConnection _dbConnection;

	/// <summary>
	/// Конструктор
	/// </summary>
	/// <param name="context">Контекст БД</param>
	/// <param name="passwordService">Сервис паролей</param>
	/// <param name="dbConnection"></param>
	public SignUpCommandHandler(
		IApplicationDbContext context,
		IPasswordService passwordService,
		IDbConnection dbConnection)
	{
		_context = context;
		_passwordService = passwordService;
		_dbConnection = dbConnection;
	}

	/// <inheritdoc/>
	public async Task<SignUpResponse> Handle(SignUpCommand request, CancellationToken cancellationToken)
	{
		var @lock = new PostgresDistributedLock(
			new PostgresAdvisoryLockKey(
				$"user_{request.Login}",
				allowHashing: true),
			_dbConnection);

		await using var handle = await @lock.TryAcquireAsync(TimeSpan.Zero, cancellationToken);

		if (handle == null)
			throw new ValidationProblem("Пользователь с таким логином уже создается");

		var isLoginUnique = await _context.Users
			.AllAsync(x => x.Login != request.Login, cancellationToken);

		if (!isLoginUnique)
			throw new ValidationProblem("Пользователь с таким логином уже существует");

		if (!Regex.IsMatch(request.Password, @"^[a-zA-Z0-9]+$"))
			throw new ValidationProblem("Для пароля запрещены все символы кроме латинских букв и цифр");

		_passwordService.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

		var user = new User(
			login: request.Login,
			passwordHash: passwordHash,
			passwordSalt: passwordSalt,
			userGroup: _context.DefaultUserGroup,
			userState: _context.ActiveUserState);

		await _context.Users.AddAsync(user, cancellationToken);

		await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
		await _context.SaveChangesAsync(cancellationToken);

		return new SignUpResponse()
		{
			UserId = user.Id,
		};
	}
}
