using HostInitActions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UserApiTestTaskVk.Application.Common.Interfaces;
using UserApiTestTaskVk.Domain.InitialEntities;
using UserApiTestTaskVk.Infrastructure.Persistence;

namespace UserApiTestTaskVk.Infrastructure.InitExecutors;

/// <summary>
/// Инициализатор базы данных
/// </summary>
public class DbInitExecutor : IAsyncInitActionExecutor
{
	private readonly IServiceScopeFactory _scopeFactory;
	private readonly IWebHostEnvironment _environment;
	private readonly IConfiguration _configuration;
	private readonly IPasswordService _passwordService;

	/// <summary>
	/// Конструктор
	/// </summary>
	/// <param name="scopeFactory">Фабрика скоупов</param>
	/// <param name="environment">Переменные среды</param>
	/// <param name="configuration">Конфигурация приложения</param>
	/// <param name="passwordService">Сервис паролей</param>
	public DbInitExecutor(
		IServiceScopeFactory scopeFactory,
		IWebHostEnvironment environment,
		IConfiguration configuration,
		IPasswordService passwordService)
	{
		_scopeFactory = scopeFactory;
		_environment = environment;
		_configuration = configuration;
		_passwordService = passwordService;
	}

	/// <summary>
	/// Провести инициализацию БД
	/// </summary>
	/// <param name="cancellationToken">Токен отмены</param>
	public async Task ExecuteAsync(CancellationToken cancellationToken)
	{
		using var scope = _scopeFactory.CreateScope();
		var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

		if (_environment.IsStaging())
		{
			await db.Database.EnsureCreatedAsync(cancellationToken);
			return;
		}

		await db.Database.MigrateAsync(cancellationToken);



		var isAdminExists = await db.Users
			.AnyAsync(u => u.Login == AdminUser.AdminLogin, cancellationToken);

		if (!isAdminExists)
		{
			_passwordService.CreatePasswordHash(
				_configuration["AppSettings:AdminPassword"],
				out var passwordHash,
				out var passwordSalt);

			await db.Users.AddAsync(new AdminUser(
				passwordHash: passwordHash,
				passwordSalt: passwordSalt),
			cancellationToken);

			await db.SaveChangesAsync(cancellationToken);
		}
	}
}
