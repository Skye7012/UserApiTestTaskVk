using System.Security.Cryptography;
using UserApiTestTaskVk.Application.Common.Interfaces;

namespace UserApiTestTaskVk.Infrastructure.Services;

/// <summary>
/// Сервис паролей
/// </summary>
public class PasswordService : IPasswordService
{
	/// <inheritdoc/>
	public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
	{
		using var hmac = new HMACSHA512();

		passwordSalt = hmac.Key;
		passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
	}

	/// <inheritdoc/>
	public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
	{
		using var hmac = new HMACSHA512(passwordSalt);

		var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
		return computedHash.SequenceEqual(passwordHash);
	}
}
