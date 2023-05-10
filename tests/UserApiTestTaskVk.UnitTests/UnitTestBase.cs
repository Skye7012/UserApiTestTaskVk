using System;
using System.Threading;
using System.Threading.Tasks;
using Medallion.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Options;
using NSubstitute;
using UserApiTestTaskVk.Application.Common.Configs;
using UserApiTestTaskVk.Application.Common.Interfaces;
using UserApiTestTaskVk.Infrastructure.Persistence;
using UserApiTestTaskVk.UnitTests.Mocks;

namespace UserApiTestTaskVk.UnitTests;

/// <summary>
/// Базовый класс для unit тестов
/// </summary>
public class UnitTestBase
{
	/// <summary>
	/// Конструктор
	/// </summary>
	protected UnitTestBase()
	{
		DateTimeProvider = Substitute.For<IDateTimeProvider>();
		PasswordService = new PasswordServiceSubstitute().Create();

		PasswordService.CreatePasswordHash("AdminPassword", out var passwordHash, out var passwordSalt);
		PasswordService.ClearReceivedCalls();
		AdminUser = new TestUser
		{
			Login = "Admin",
			Password = "AdminPassword",
			PasswordHash = passwordHash,
			PasswordSalt = passwordSalt,
		};


		AuthorizationService = new AuthorizationServiceSubstitute(AdminUser).Create();
		TokenService = new TokenServiceSubstitute(AdminUser).Create();


		DistributedLockProvider = Substitute.For<IDistributedLockProvider>();

		ConfigureDateTimeProvider();
		ConfigureDistributedLockProvider();


		LockDelaysConfig = Options.Create(new LockDelaysConfig
		{
			UserLockDelay = 0
		});
	}

	/// <summary>
	/// Сервис авторизации для тестов
	/// </summary>
	protected IAuthorizationService AuthorizationService { get; }

	/// <summary>
	/// Сервис JWT токенов для тестов
	/// </summary>
	protected ITokenService TokenService { get; }

	/// <summary>
	/// Сервис паролей для тестов
	/// </summary>
	protected IPasswordService PasswordService { get; }

	/// <summary>
	/// Провайдер времени
	/// </summary>
	protected IDateTimeProvider DateTimeProvider { get; }

	/// <summary>
	/// Провайдер локов
	/// </summary>
	protected IDistributedLockProvider DistributedLockProvider { get; }

	/// <summary>
	/// Конфигурация для задержек локов
	/// </summary>
	protected IOptions<LockDelaysConfig> LockDelaysConfig { get; }

	/// <summary>
	/// Пользователь-администратор, который присутствует в тестах в БД по дефолту
	/// </summary>
	public TestUser AdminUser { get; }

	/// <summary>
	/// Создать контекст БД
	/// </summary>
	/// <returns>Контекст БД</returns>
	protected IApplicationDbContext CreateInMemoryContext(Action<IApplicationDbContext>? seedActions = null)
	{
		var context = new ApplicationDbContext(
			new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(Guid.NewGuid().ToString())
				.ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
				.Options,
			DateTimeProvider);

		context.Database.EnsureCreated();

		AdminUser.UserGroup = context.AdminUserGroup;
		AdminUser.UserState = context.ActiveUserState;

		context.Users.Add(AdminUser);

		seedActions?.Invoke(context);

		context.SaveChanges();
		return context;
	}

	/// <summary>
	/// Сконфигурировать <see cref="DateTimeProvider"/>
	/// </summary>
	private void ConfigureDateTimeProvider()
	{
		DateTimeProvider.UtcNow
			.Returns(DateTime.SpecifyKind(
				new DateTime(2020, 01, 01),
				DateTimeKind.Utc));
	}

	/// <summary>
	/// Сконфигурировать <see cref="DistributedLockProvider"/>
	/// </summary>
	private void ConfigureDistributedLockProvider()
	{
		var distributedLock = Substitute.For<IDistributedLock>();
		distributedLock.TryAcquireAsync(Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>())
			.Returns(ValueTask.FromResult(Substitute.For<IDistributedSynchronizationHandle?>()));

		DistributedLockProvider.CreateLock(Arg.Any<string>())
			.Returns(distributedLock);
	}
}
