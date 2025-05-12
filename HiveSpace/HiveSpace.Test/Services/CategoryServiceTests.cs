using HiveSpace.Application.Helpers;
using HiveSpace.Application.Interfaces;
using HiveSpace.Application.Models.ViewModels;
using HiveSpace.Application.Queries;
using HiveSpace.Application.Services;
using Moq;

namespace HiveSpace.Test.Services;

public class CategoryServiceTests
{
    private readonly Mock<IQueryService> _mockQueryService;
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly CategoryService _categoryService;

    public CategoryServiceTests()
    {
        _mockQueryService = new Mock<IQueryService>();
        _mockCacheService = new Mock<ICacheService>();
        _categoryService = new CategoryService(_mockQueryService.Object, _mockCacheService.Object);
    }

    [Fact]
    public async Task GetCategoryAsync_ShouldReturnCategoriesFromCache_WhenCacheHasData()
    {
        // Arrange
        var expectedCategories = new List<CategoryViewModel>
        {
            new() { Id = 1, Name = "Category 1" },
            new() { Id = 2, Name = "Category 2" }
        };

        _mockCacheService
            .Setup(x => x.GetOrCreateAsync(
                It.Is<string>(key => key == CacheKeys.Categories),
                It.IsAny<Func<Task<List<CategoryViewModel>>>>(),
                null))
            .ReturnsAsync(expectedCategories);

        // Act
        var result = await _categoryService.GetCategoryAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedCategories.Count, result.Count);
        Assert.Equal(expectedCategories[0].Name, result[0].Name);
        Assert.Equal(expectedCategories[1].Name, result[1].Name);
        _mockQueryService.Verify(x => x.GetCategoryViewModelsAsync(), Times.Never);
    }

    [Fact]
    public async Task GetCategoryAsync_ShouldCallQueryService_WhenCacheIsEmpty()
    {
        // Arrange
        var expectedCategories = new List<CategoryViewModel>
        {
            new() { Id = 1, Name = "Category 1" }
        };

        _mockQueryService
            .Setup(x => x.GetCategoryViewModelsAsync())
            .ReturnsAsync(expectedCategories);

        Func<Task<List<CategoryViewModel>>> capturedCallback = null;

        _mockCacheService
            .Setup(x => x.GetOrCreateAsync(
                It.Is<string>(key => key == CacheKeys.Categories),
                It.IsAny<Func<Task<List<CategoryViewModel>>>>(),
                null))
            .Callback<string, Func<Task<List<CategoryViewModel>>>, TimeSpan?>((key, callback, expiry) =>
            {
                capturedCallback = callback;
            })
            .ReturnsAsync((string key, Func<Task<List<CategoryViewModel>>> callback, TimeSpan? expiry) =>
            {
                return callback().Result;
            });

        // Act
        var result = await _categoryService.GetCategoryAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(expectedCategories[0].Name, result[0].Name);
        _mockQueryService.Verify(x => x.GetCategoryViewModelsAsync(), Times.Once);
    }
}