using System.Data;
using HostInitActions;
using Medallion.Threading;
using Medallion.Threading.Postgres;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using UserApiTestTaskVk.Application.Common.Configs;
using UserApiTestTaskVk.Application.Common.Extensions;
using UserApiTestTaskVk.Application.Common.Interfaces;
using UserApiTestTaskVk.Infrastructure.InitExecutors;
using UserApiTestTaskVk.Infrastructure.Persistence;
using UserApiTestTaskVk.Infrastructure.Services;

namespace UserApiTestTaskVk.Infrastructure;

/// <summary>
/// Конфигуратор сервисов
/// </summary>
public static class InfrastructureServicesConfigurator
{
	/// <summary>
	/// Сконфигурировать сервисы
	/// </summary>
	/// <param name="services">Билдер приложения</param>
	/// <param name="configuration">Конфигурации приложения</param>
	public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
		=> services
			.AddAuthorization(configuration)
			.AddDatabase(configuration)
			.AddInitExecutors()
			.AddDistributedLockProvider(configuration);

	/// <summary>
	/// Сконфигурировать сервисы авторизации
	/// </summary>
	/// <param name="services">Сервисы</param>
	/// <param name="configuration">Конфигурации приложения</param>
	private static IServiceCollection AddAuthorization(this IServiceCollection services, IConfiguration configuration)
	{
		services
			.AddTransient<IPasswordService, PasswordService>()
			.AddTransient<ITokenService, TokenService>()
			.AddTransient<IAuthorizationService, AuthorizationService>();

		var jwtConfig = services.ConfigureAndGet<JwtConfig>(
			configuration,
			JwtConfig.ConfigSectionName);

		var tokenValidationParameters = jwtConfig.BuildTokenValidationParameters();

		services
			.AddSingleton(tokenValidationParameters)
			.AddTransient<IRefreshTokenValidator, RefreshTokenValidator>();

		services
			.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer(options => options.TokenValidationParameters = tokenValidationParameters);

		services.AddAuthorization();

		return services;
	}

	/// <summary>
	/// Сконфигурировать подключение к БД
	/// </summary>
	/// <param name="services">Сервисы</param>
	/// <param name="configuration">Конфигурации приложения</param>
	private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
	{
		var connString = configuration.GetConnectionString("Db")!;
		services.AddDbContext<ApplicationDbContext>(opt =>
			{
				opt.UseNpgsql(connString);
				opt.UseSnakeCaseNamingConvention();
			});

		services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

		services.AddTransient<IDbConnection>(_ =>
		{
			var dbConnection = new NpgsqlConnection(connString);
			dbConnection.Open();
			return dbConnection;
		});

		return services;
	}

	/// <summary>
	/// Сконфигурировать инициализаторы сервисов
	/// </summary>
	/// <param name="services">Сервисы</param>
	private static IServiceCollection AddInitExecutors(this IServiceCollection services)
	{
		services.AddAsyncServiceInitialization()
			.AddInitActionExecutor<DbInitExecutor>();

		return services;
	}

	/// <summary>
	/// Сконфигурировать провайдер локов
	/// </summary>
	/// <param name="services">Сервисы</param>
	/// <param name="configuration">Конфигурации приложения</param>
	private static IServiceCollection AddDistributedLockProvider(this IServiceCollection services, IConfiguration configuration)
	{
		var lockDelaysConfig = services.Configure<LockDelaysConfig>(
			configuration.GetSection(JwtConfig.ConfigSectionName));

		return services.AddSingleton<IDistributedLockProvider>(sp =>
		{
			var dbConnection = sp.GetRequiredService<IDbConnection>();
			return new PostgresDistributedSynchronizationProvider(dbConnection);
		});
	}
}
