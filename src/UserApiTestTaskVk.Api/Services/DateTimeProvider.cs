using UserApiTestTaskVk.Application.Common.Interfaces;

namespace UserApiTestTaskVk.Api.Services;

/// <inheritdoc/>
public class DateTimeProvider : IDateTimeProvider
{
	/// <inheritdoc/>
	public DateTime UtcNow => DateTime.UtcNow;
}
