using System.Text.RegularExpressions;
using Medallion.Threading;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using UserApiTestTaskVk.Application.Common.Configs;
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
	private readonly IDistributedLockProvider _lockProvider;
	private readonly LockDelaysConfig _lockDelaysConfing;

	/// <summary>
	/// Конструктор
	/// </summary>
	/// <param name="context">Контекст БД</param>
	/// <param name="passwordService">Сервис паролей</param>
	/// <param name="lockProvider">Провайдер локов</param>
	/// <param name="lockDelaysConfing">Конфигурация для задержек локов</param>
	public SignUpCommandHandler(
		IApplicationDbContext context,
		IPasswordService passwordService,
		IDistributedLockProvider lockProvider,
		IOptions<LockDelaysConfig> lockDelaysConfing)
	{
		_context = context;
		_passwordService = passwordService;
		_lockProvider = lockProvider;
		_lockDelaysConfing = lockDelaysConfing.Value;
	}

	/// <inheritdoc/>
	public async Task<SignUpResponse> Handle(SignUpCommand request, CancellationToken cancellationToken)
	{
		await using var handle = await _lockProvider.TryAcquireLockAsync(User.GetLockKey(request.Login));

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

		await Task.Delay(_lockDelaysConfing.UserLockDelay, cancellationToken);
		await _context.SaveChangesAsync(cancellationToken);

		return new SignUpResponse()
		{
			UserId = user.Id,
		};
	}
}
