using System.Text.RegularExpressions;
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

	/// <summary>
	/// Конструктор
	/// </summary>
	/// <param name="context">Контекст БД</param>
	/// <param name="passwordService">Сервис паролей</param>
	public SignUpCommandHandler(
		IApplicationDbContext context,
		IPasswordService passwordService)
	{
		_context = context;
		_passwordService = passwordService;
	}

	/// <inheritdoc/>
	public async Task<SignUpResponse> Handle(SignUpCommand request, CancellationToken cancellationToken)
	{
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
			passwordSalt: passwordSalt);

		await _context.Users.AddAsync(user, cancellationToken);
		await _context.SaveChangesAsync(cancellationToken);

		return new SignUpResponse()
		{
			UserId = user.Id,
		};
	}
}
