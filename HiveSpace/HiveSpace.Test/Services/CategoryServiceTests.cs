using HiveSpace.Application.Helpers;               // Các helper chung trong ứng dụng
using HiveSpace.Application.Interfaces;            // Interface các service, repo
using HiveSpace.Application.Models.ViewModels;      // ViewModel dùng trả về client
using HiveSpace.Application.Queries;                 // Các query service
using HiveSpace.Application.Services;                // Service cần test
using Moq;                                           // Thư viện mock để viết unit test

namespace HiveSpace.Test.Services;

public class CategoryServiceTests
{
    // Mock các dependency của CategoryService
    private readonly Mock<IQueryService> _mockQueryService;
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly CategoryService _categoryService;  // Đối tượng service thật để test

    // Constructor khởi tạo mock và đối tượng service
    public CategoryServiceTests()
    {
        _mockQueryService = new Mock<IQueryService>();
        _mockCacheService = new Mock<ICacheService>();
        _categoryService = new CategoryService(_mockQueryService.Object, _mockCacheService.Object);
    }

    // Test: khi cache có dữ liệu, GetCategoryAsync trả về dữ liệu từ cache mà không gọi query service
    [Fact]
    public async Task GetCategoryAsync_ShouldReturnCategoriesFromCache_WhenCacheHasData()
    {
        // Arrange: tạo danh sách category giả lập trả về từ cache
        var expectedCategories = new List<CategoryViewModel>
        {
            new() { Id = 1, Name = "Category 1" },
            new() { Id = 2, Name = "Category 2" }
        };

        // Setup mock cacheService: khi gọi GetOrCreateAsync với key đúng, trả về danh sách giả lập
        _mockCacheService
            .Setup(x => x.GetOrCreateAsync(
                It.Is<string>(key => key == CacheKeys.Categories),    // key đúng
                It.IsAny<Func<Task<List<CategoryViewModel>>>>(),      // callback bất kỳ
                null))
            .ReturnsAsync(expectedCategories);

        // Act: gọi hàm GetCategoryAsync của service
        var result = await _categoryService.GetCategoryAsync();

        // Assert: kiểm tra kết quả không null, đúng số phần tử, tên đúng
        Assert.NotNull(result);
        Assert.Equal(expectedCategories.Count, result.Count);
        Assert.Equal(expectedCategories[0].Name, result[0].Name);
        Assert.Equal(expectedCategories[1].Name, result[1].Name);
        // Kiểm tra queryService không được gọi (vì lấy từ cache)
        _mockQueryService.Verify(x => x.GetCategoryViewModelsAsync(), Times.Never);
    }

    // Test: khi cache trống, GetCategoryAsync gọi query service lấy dữ liệu và cache lại
    [Fact]
    public async Task GetCategoryAsync_ShouldCallQueryService_WhenCacheIsEmpty()
    {
        // Arrange: tạo danh sách category giả lập trả về từ query service
        var expectedCategories = new List<CategoryViewModel>
        {
            new() { Id = 1, Name = "Category 1" }
        };

        // Setup mock queryService trả về danh sách giả lập
        _mockQueryService
            .Setup(x => x.GetCategoryViewModelsAsync())
            .ReturnsAsync(expectedCategories);

        Func<Task<List<CategoryViewModel>>> capturedCallback = null;

        // Setup mock cacheService để bắt callback và gọi callback thực sự để lấy dữ liệu
        _mockCacheService
            .Setup(x => x.GetOrCreateAsync(
                It.Is<string>(key => key == CacheKeys.Categories),
                It.IsAny<Func<Task<List<CategoryViewModel>>>>(),
                null))
            .Callback<string, Func<Task<List<CategoryViewModel>>>, TimeSpan?>((key, callback, expiry) =>
            {
                capturedCallback = callback;  // Lưu callback để gọi sau
            })
            .ReturnsAsync((string key, Func<Task<List<CategoryViewModel>>> callback, TimeSpan? expiry) =>
            {
                return callback().Result;    // Thực sự gọi callback để lấy dữ liệu
            });

        // Act: gọi hàm lấy category
        var result = await _categoryService.GetCategoryAsync();

        // Assert: kiểm tra kết quả hợp lệ, số lượng đúng
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(expectedCategories[0].Name, result[0].Name);
        // Kiểm tra queryService được gọi đúng 1 lần
        _mockQueryService.Verify(x => x.GetCategoryViewModelsAsync(), Times.Once);
    }
}
