using HiveSpace.Application.Models.AppSettings; // Import model cấu hình app
using HiveSpace.Application.Services;           // Import service CacheService
using HiveSpace.Domain.Enums;                   // Import các enum trong domain
using Microsoft.Extensions.Caching.Memory;      // Bộ nhớ đệm nội bộ (InMemoryCache)
using Microsoft.Extensions.Options;             // Lấy cấu hình Options pattern
using Moq;                                      // Thư viện mock đối tượng dùng cho unit test
using StackExchange.Redis;                      // Thư viện làm việc với Redis
using System.Text.Json;                         // Thư viện serialize/deserialize JSON

namespace HiveSpace.Test.Services;

public class CacheServiceTests
{
    // Tạo các biến mock (giả lập) để thay thế các thành phần thật trong test
    private readonly Mock<IMemoryCache> _mockMemoryCache;  // Mock bộ nhớ đệm nội bộ
    private readonly Mock<IDatabase> _mockRedisDb;         // Mock database Redis
    private readonly Mock<IOptions<RedisOption>> _mockRedisOptions;  // Mock cấu hình Redis Options
    private readonly CacheService _service;                 // Đối tượng CacheService thật sẽ test
    private readonly string _instanceName = "test";         // Tên instance dùng để tiền tố key cache
    private readonly TimeSpan _defaultExpiration = TimeSpan.FromMinutes(30); // Thời gian hết hạn mặc định cho cache

    // Constructor của class test, sẽ chạy trước các test method
    public CacheServiceTests()
    {
        _mockMemoryCache = new Mock<IMemoryCache>();  // Khởi tạo mock cho bộ nhớ đệm
        _mockRedisDb = new Mock<IDatabase>();          // Khởi tạo mock cho Redis DB
        _mockRedisOptions = new Mock<IOptions<RedisOption>>();  // Khởi tạo mock cho cấu hình

        // Setup mock cấu hình trả về giá trị RedisOption với InstanceName và DefaultExpiration
        _mockRedisOptions.Setup(x => x.Value).Returns(new RedisOption
        {
            InstanceName = _instanceName,
            DefaultExpiration = _defaultExpiration
        });

        // Tạo đối tượng CacheService thật sử dụng các mock ở trên
        _service = new CacheService(
            _mockMemoryCache.Object,  // truyền mock bộ nhớ đệm thật
            _mockRedisDb.Object,      // truyền mock Redis DB thật
            _mockRedisOptions.Object  // truyền mock Options thật
        );
    }

    // Test method: kiểm tra khi dữ liệu có trong bộ nhớ đệm nội bộ thì trả về luôn
    [Fact]
    public async Task GetAsync_WhenInMemoryCache_ReturnsValue()
    {
        // Arrange: chuẩn bị các biến cần thiết cho test
        var key = "testKey";                   // key test
        var expectedValue = "testValue";      // giá trị dự kiến trả về
        var prefixedKey = $"{_instanceName}:{key}"; // key có tiền tố để phân biệt cache

        object? cachedValue = expectedValue;  // mock trả về giá trị này khi đọc cache
        // Setup mock bộ nhớ đệm, khi gọi TryGetValue với key trên sẽ trả true và giá trị cachedValue
        _mockMemoryCache.Setup(x => x.TryGetValue(prefixedKey, out cachedValue))
            .Returns(true);

        // Act: gọi hàm GetAsync của CacheService để lấy dữ liệu cache
        var result = await _service.GetAsync<string>(key);

        // Assert: kiểm tra kết quả trả về bằng giá trị dự kiến
        Assert.Equal(expectedValue, result);
    }

    // Test method: kiểm tra khi không có trong bộ nhớ đệm nhưng có trong Redis thì trả về đúng
    [Fact]
    public async Task GetAsync_WhenInRedisCache_ReturnsValue()
    {
        // Arrange
        var key = "testKey";
        var expectedValue = "testValue";
        var prefixedKey = $"{_instanceName}:{key}";

        object? cachedValue = null;  // trong bộ nhớ đệm không có dữ liệu (null)
        _mockMemoryCache.Setup(x => x.TryGetValue(prefixedKey, out cachedValue))
            .Returns(false);  // trả false, không tìm thấy

        // Dữ liệu Redis trả về ở dạng chuỗi JSON
        var serializedValue = JsonSerializer.Serialize(expectedValue);
        // Setup mock Redis trả về giá trị serialize này khi gọi StringGetAsync
        _mockRedisDb.Setup(x => x.StringGetAsync(It.IsAny<RedisKey>(), CommandFlags.None))
            .ReturnsAsync(serializedValue);

        // Setup mock CreateEntry cho memory cache
        var mockEntry = new Mock<ICacheEntry>();
        _mockMemoryCache.Setup(x => x.CreateEntry(It.IsAny<object>()))
            .Returns(mockEntry.Object);

        // Act: gọi hàm lấy dữ liệu cache
        var result = await _service.GetAsync<string>(key);

        // Assert: kiểm tra kết quả trả về đúng
        Assert.Equal(expectedValue, result);
    }

    // Test method: kiểm tra khi không tìm thấy dữ liệu trong cả 2 cache thì trả về null
    [Fact]
    public async Task GetAsync_WhenNotInCache_ReturnsNull()
    {
        // Arrange
        var key = "testKey";
        var prefixedKey = $"{_instanceName}:{key}";

        object? cachedValue = null;
        // Bộ nhớ đệm nội bộ trả về không có dữ liệu
        _mockMemoryCache.Setup(x => x.TryGetValue(prefixedKey, out cachedValue))
            .Returns(false);

        // Redis trả về giá trị null (không tìm thấy)
        _mockRedisDb.Setup(x => x.StringGetAsync(It.IsAny<RedisKey>(), CommandFlags.None))
            .ReturnsAsync(RedisValue.Null);

        // Act: gọi hàm và kiểm tra nó trả về null
        var result = await _service.GetAsync<string>(key);

        // Assert: kết quả phải là null
        Assert.Null(result);
    }

    // Test method: kiểm tra khi gọi SetAsync thì lưu dữ liệu vào cả bộ nhớ đệm nội bộ và Redis
    [Fact]
    public async Task SetAsync_WhenCalled_SetsInBothCaches()
    {
        // Arrange
        var key = "testKey";
        var value = "testValue";

        // Setup Redis trả về true khi gọi lưu dữ liệu (5 parameter signature)
        _mockRedisDb.Setup(x => x.StringSetAsync(
            It.IsAny<RedisKey>(),
            It.IsAny<RedisValue>(),
            It.IsAny<TimeSpan?>(),
            When.Always,
            CommandFlags.None))
            .ReturnsAsync(true);

        // Setup mock CreateEntry cho memory cache
        var mockEntry = new Mock<ICacheEntry>();
        _mockMemoryCache.Setup(x => x.CreateEntry(It.IsAny<object>()))
            .Returns(mockEntry.Object);

        // Act: gọi hàm lưu dữ liệu cache
        var result = await _service.SetAsync(key, value);

        // Assert: kiểm tra hàm trả về true (lưu thành công)
        Assert.True(result);
    }

    // Test method: kiểm tra khi gọi RemoveAsync thì xóa dữ liệu khỏi cả bộ nhớ đệm nội bộ và Redis
    [Fact]
    public async Task RemoveAsync_WhenCalled_RemovesFromBothCaches()
    {
        // Arrange
        var key = "testKey";

        // Setup Redis trả về true khi xóa key thành công
        _mockRedisDb.Setup(x => x.KeyDeleteAsync(It.IsAny<RedisKey>(), CommandFlags.None))
            .ReturnsAsync(true);

        // Act: gọi hàm xóa cache
        var result = await _service.RemoveAsync(key);

        // Assert: kiểm tra hàm trả về true (xóa thành công)
        Assert.True(result);
    }

    // Test method: kiểm tra khi GetOrCreateAsync có dữ liệu trong bộ nhớ đệm thì trả về luôn
    [Fact]
    public async Task GetOrCreateAsync_WhenInCache_ReturnsCachedValue()
    {
        // Arrange
        var key = "testKey";
        var expectedValue = "testValue";
        var prefixedKey = $"{_instanceName}:{key}";

        object? cachedValue = expectedValue;
        // Setup bộ nhớ đệm trả về dữ liệu cachedValue
        _mockMemoryCache.Setup(x => x.TryGetValue(prefixedKey, out cachedValue))
            .Returns(true);

        // Act: gọi GetOrCreateAsync với callback trả về "newValue" (nhưng không dùng)
        var result = await _service.GetOrCreateAsync(key, () => Task.FromResult("newValue"));

        // Assert: kết quả trả về là giá trị trong cache
        Assert.Equal(expectedValue, result);
    }

    // Test method: kiểm tra GetOrCreateAsync khi không có dữ liệu thì tạo mới, lưu cache và trả về
    [Fact]
    public async Task GetOrCreateAsync_WhenNotInCache_CreatesAndCachesValue()
    {
        // Arrange
        var key = "testKey";
        var newValue = "newValue";
        var prefixedKey = $"{_instanceName}:{key}";

        object? cachedValue = null;
        // Bộ nhớ đệm không có dữ liệu
        _mockMemoryCache.Setup(x => x.TryGetValue(prefixedKey, out cachedValue))
            .Returns(false);

        // Redis cũng không có dữ liệu
        _mockRedisDb.Setup(x => x.StringGetAsync(It.IsAny<RedisKey>(), CommandFlags.None))
            .ReturnsAsync(RedisValue.Null);

        // Setup Redis lưu dữ liệu trả về true (5 parameter signature)
        _mockRedisDb.Setup(x => x.StringSetAsync(
            It.IsAny<RedisKey>(),
            It.IsAny<RedisValue>(),
            It.IsAny<TimeSpan?>(),
            When.Always,
            CommandFlags.None))
            .ReturnsAsync(true);

        // Setup mock CreateEntry cho memory cache
        var mockEntry = new Mock<ICacheEntry>();
        _mockMemoryCache.Setup(x => x.CreateEntry(It.IsAny<object>()))
            .Returns(mockEntry.Object);

        // Act: gọi GetOrCreateAsync với callback trả về newValue
        var result = await _service.GetOrCreateAsync(key, () => Task.FromResult(newValue));

        // Assert: kết quả đúng giá trị tạo mới
        Assert.Equal(newValue, result);
    }
}
