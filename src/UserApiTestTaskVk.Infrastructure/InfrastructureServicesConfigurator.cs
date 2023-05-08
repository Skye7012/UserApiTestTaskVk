using System.Text;
using HostInitActions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using UserApiTestTaskVk.Application.Common.Extensions;
using UserApiTestTaskVk.Application.Common.Interfaces;
using UserApiTestTaskVk.Infrastructure.Configs;
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
			.AddInitExecutors();

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

		var tokenValidationParameters = new TokenValidationParameters
		{
			IssuerSigningKey = new SymmetricSecurityKey(
				Encoding.UTF8.GetBytes(jwtConfig.Key)),
			ValidIssuer = jwtConfig.Issuer,
			ValidAudience = jwtConfig.Audience,
			ValidateIssuerSigningKey = true,
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ClockSkew = TimeSpan.Zero,
		};

		services.AddSingleton(tokenValidationParameters);

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

		return services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
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
}
