using System.Linq;
using FluentAssertions;
using UserApiTestTaskVk.Application.Common.Extensions;
using UserApiTestTaskVk.Contracts.Requests.Common;
using UserApiTestTaskVk.Domain.Entities;
using Xunit;

namespace UserApiTestTaskVk.UnitTests.Extensions;

/// <summary>
/// Тесты для <see cref="QueryableExtension.Sort"/>
/// </summary>
public class IQueryableSortTests : UnitTestBase
{
	/// <summary>
	/// Должен правильно отсортировать при HappyPath
	/// </summary>
	[Fact]
	public void Sort_ShouldProperlySort_WhenHappyPath()
	{
		var generateToken = (int n) => new RefreshToken(n.ToString(), AdminUser);
		var context = CreateInMemoryContext(x => x.RefreshTokens.AddRange(
			Enumerable.Range(1, 8).Select(i => generateToken(i))));

		var filter = new BaseGetRequest()
		{
			IsAscending = true,
			Limit = 4,
			Page = 2,
			OrderBy = nameof(RefreshToken.Token),
		};

		var res = context.RefreshTokens
			.Sort(filter)
			.ToList();

		res.Should().HaveCount(4);
		res.First().Token.Should().Be("5");
		res.Last().Token.Should().Be("8");
	}

	/// <summary>
	/// Должен правильно отсортировать с сортировкой по убыванию и количеством записей меньше лимита
	/// </summary>
	[Fact]
	public void Sort_ShouldProperlySort_WithDescendingAndCountLessThanLimit()
	{
		var generateToken = (int n) => new RefreshToken(n.ToString(), AdminUser);
		var context = CreateInMemoryContext(x => x.RefreshTokens.AddRange(
			Enumerable.Range(1, 3).Select(i => generateToken(i))));

		var filter = new BaseGetRequest()
		{
			IsAscending = false,
			Limit = 4,
			Page = 1,
			OrderBy = nameof(RefreshToken.Token),
		};

		var res = context.RefreshTokens
			.Sort(filter)
			.ToList();

		res.Should().HaveCount(3);
		res.First().Token.Should().Be("3");
		res.Last().Token.Should().Be("1");
	}

	/// <summary>
	/// Должен вернуть пустой массив, когда смещение больше кол-ва записей
	/// </summary>
	[Fact]
	public void Sort_ShouldReturnEmpty_WhenOffsetMoreThanItems()
	{
		var generateToken = (int n) => new RefreshToken(n.ToString(), AdminUser);
		var context = CreateInMemoryContext(x => x.RefreshTokens.AddRange(
			Enumerable.Range(1, 4).Select(i => generateToken(i))));

		var filter = new BaseGetRequest()
		{
			IsAscending = false,
			Limit = 4,
			Page = 2,
			OrderBy = nameof(RefreshToken.Token),
		};

		var res = context.RefreshTokens
			.Sort(filter)
			.ToList();

		res.Should().BeEmpty();
	}
}
