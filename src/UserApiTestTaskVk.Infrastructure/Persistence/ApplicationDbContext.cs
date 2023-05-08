using Microsoft.EntityFrameworkCore;
using UserApiTestTaskVk.Application.Common.Interfaces;
using UserApiTestTaskVk.Domain.Entities;
using UserApiTestTaskVk.Domain.Entities.Common;
using UserApiTestTaskVk.Domain.InitialEntities;

namespace UserApiTestTaskVk.Infrastructure.Persistence;

/// <summary>
/// Контекст БД данного приложения
/// </summary>
public class ApplicationDbContext : DbContext, IApplicationDbContext
{
	private readonly IAuthorizationService _authorizationService;
	private readonly IDateTimeProvider _dateTimeProvider;

	/// <summary>
	/// Конструктор
	/// </summary>
	/// <param name="options">Опции контекста</param>
	/// <param name="authorizationService">Сервис авторизации</param>
	/// <param name="dateTimeProvider">Провайдер даты и времени</param>
	public ApplicationDbContext(
		DbContextOptions<ApplicationDbContext> options,
		IAuthorizationService authorizationService,
		IDateTimeProvider dateTimeProvider)
		: base(options)
	{
		_authorizationService = authorizationService;
		_dateTimeProvider = dateTimeProvider;

		AttachInitialEntitiesToContext();
	}

	/// <inheritdoc/>
	public DbContext Instance => this;

	/// <summary>
	/// Пользователи
	/// </summary>
	public DbSet<User> Users { get; private set; } = default!;

	/// <summary>
	/// Группы пользователей
	/// </summary>
	public DbSet<UserGroup> UserGroups { get; private set; } = default!;

	/// <summary>
	/// Статусы пользователей
	/// </summary>
	public DbSet<UserState> UserStates { get; private set; } = default!;

	/// <summary>
	/// Refresh токены
	/// </summary>
	public DbSet<RefreshToken> RefreshTokens { get; private set; } = default!;

	/// <inheritdoc/>
	protected override void OnModelCreating(ModelBuilder modelBuilder) 
		=> modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

	/// <inheritdoc/>
	public async Task<int> SaveChangesAsync(
		bool withSoftDelete = true,
		bool acceptAllChangesOnSuccess = true,
		CancellationToken cancellationToken = default)
	{
		HandleSaveChangesLogic(withSoftDelete);

		return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
	}

	/// <summary>
	/// Сохранить изменения
	/// </summary>
	/// <param name="cancellationToken">Токен отмены</param>
	/// <returns>Количество записей состояния, записанных в базу данных</returns>
	public async override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		HandleSaveChangesLogic(true);

		return await base.SaveChangesAsync(cancellationToken);
	}

	/// <summary>
	/// Обработать логику сохранения изменений
	/// </summary>
	/// <param name="withSoftDelete">Использовать мягкое удаление</param>
	private void HandleSaveChangesLogic(bool withSoftDelete)
	{
		if (!_authorizationService.IsAuthenticated())
		{
			HandleRefreshTokenAddingAndRemovingWhenSignIn();
			return;
		}

		HandleEntityBaseCreatedMetadata();

		HandleUserSoftDelete();

		if (!withSoftDelete)
			return;

		HandleSoftDelete();
	}

	/// <summary>
	/// Обработать refresh токены при аутентификации
	/// (их добавлении и удалении при привышении лимита)
	/// </summary>
	private void HandleRefreshTokenAddingAndRemovingWhenSignIn()
	{
		var refreshTokens = ChangeTracker.Entries<RefreshToken>();

		if (refreshTokens?.Any() == true)
		{
			var refreshTokenToAdd = refreshTokens
				.Where(c => c.State == EntityState.Added)
				.SingleOrDefault();

			if (refreshTokenToAdd is null)
				return;

			refreshTokenToAdd.Entity.CreatedDate = _dateTimeProvider.UtcNow;

			var refreshTokenToRemove = refreshTokens
				.Where(c => c.State == EntityState.Deleted)
				.SingleOrDefault();

			if (refreshTokenToRemove is not null)
			{
				refreshTokenToRemove.Entity.RevokedOn = _dateTimeProvider.UtcNow;
				refreshTokenToRemove.State = EntityState.Modified;
			}
		}
	}

	/// <summary>
	/// Обработать метаданные добавления у базовых сущностей
	/// </summary>
	private void HandleEntityBaseCreatedMetadata()
	{
		var changeSet = ChangeTracker.Entries<EntityBase>();

		if (changeSet?.Any() != true)
			return;

		foreach (var entry in changeSet.Where(c => c.State == EntityState.Added))
		{
			entry.Entity.CreatedDate = _dateTimeProvider.UtcNow;
		}
	}

	/// <summary>
	/// Обработать мягкое удаления пользователя
	/// </summary>
	private void HandleUserSoftDelete()
	{
		var deleteUserChangeSet = ChangeTracker.Entries<User>()
			.Where(x => x.State == EntityState.Deleted);

		if (deleteUserChangeSet?.Any() != true)
			return;

		foreach (var entry in deleteUserChangeSet)
		{
			entry.Entity.Block();
			entry.State = EntityState.Modified;
		}
	}

	/// <summary>
	/// Обработать мягкое удаления сущностей
	/// </summary>
	private void HandleSoftDelete()
	{
		var deleteChangeSet = ChangeTracker.Entries<ISoftDeletable>();

		if (deleteChangeSet?.Any() == true)
		{
			foreach (var entry in deleteChangeSet.Where(c => c.State == EntityState.Deleted))
			{
				entry.Entity.RevokedOn = _dateTimeProvider.UtcNow;
				entry.State = EntityState.Modified;
			}
		}
	}

	/// <summary>
	/// Загрузить в контекст начальные константные сущности
	/// </summary>
	private void AttachInitialEntitiesToContext()
	{
		UserGroups.AttachRange(ConstEntities.AdminUserGroup, ConstEntities.DefaultUserGroup);
		UserStates.AttachRange(ConstEntities.ActiveUserState, ConstEntities.BlockedUserState);
	}
}
