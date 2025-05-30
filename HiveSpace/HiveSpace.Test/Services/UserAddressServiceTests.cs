using AutoMapper;                                // Thư viện AutoMapper dùng để map object
using HiveSpace.Application.Interfaces;          // Interface các service, repo
using HiveSpace.Application.Models.Dtos.Request.UserAddress;  // DTO request địa chỉ người dùng
using HiveSpace.Application.Services;             // Service UserAddressService cần test
using HiveSpace.Common.Interface;                  // Interface chung
using HiveSpace.Domain.AggergateModels.UserAggregate; // Aggregate User domain
using HiveSpace.Domain.Repositories;               // Repository interfaces
using Moq;                                          // Thư viện mock để viết unit test

namespace HiveSpace.Test.Services;

public class UserAddressServiceTests
{
    // Khai báo các mock để giả lập các dependency
    private readonly Mock<IUserContext> _mockUserContext;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ICacheService> _mockRedisService;
    private readonly UserAddressService _service;  // Đối tượng service thật để test
    private readonly Guid _userId = Guid.NewGuid();  // ID user giả lập

    // Constructor khởi tạo mock và đối tượng service cần test
    public UserAddressServiceTests()
    {
        _mockUserContext = new Mock<IUserContext>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockRedisService = new Mock<ICacheService>();

        // Setup mock UserContext trả về userId giả lập
        _mockUserContext.Setup(x => x.UserId).Returns(_userId);

        // Tạo đối tượng service thật sử dụng các mock đã tạo
        _service = new UserAddressService(
            _mockUserRepository.Object,
            _mockUserContext.Object,
            _mockMapper.Object,
            _mockRedisService.Object
        );
    }

    // Test lấy danh sách địa chỉ khi user tồn tại
    [Fact]
    public async Task GetUserAddressAsync_WhenUserExists_ReturnsAddresses()
    {
        // Arrange: tạo user giả lập
        var user = new User(
            phoneNumber: "84987654321",
            password: "hashedPassword123",
            userName: "testuser",
            fullName: "Test User",
            email: "test@example.com"
        );
        user.GetType().GetProperty("Id").SetValue(user, _userId);  // Gán Id user qua reflection

        var addresses = new List<UserAddress>();        // Danh sách địa chỉ domain giả lập
        var expectedDtos = new List<UserAddressDto>();  // Danh sách DTO mong muốn trả về

        // Setup repo trả về user giả lập
        _mockUserRepository.Setup(x => x.GetByIdAsync(_userId, true))
            .ReturnsAsync(user);
        // Setup mapper trả về DTO tương ứng
        _mockMapper.Setup(x => x.Map<List<UserAddressDto>>(addresses))
            .Returns(expectedDtos);
        // Setup cacheService trả về DTO khi gọi GetOrCreateAsync, đồng thời callback factory function
        _mockRedisService.Setup(x => x.GetOrCreateAsync(
            It.IsAny<string>(),
            It.IsAny<Func<Task<List<UserAddressDto>>>>(),
            It.IsAny<TimeSpan?>()))
            .Callback<string, Func<Task<List<UserAddressDto>>>, TimeSpan?>((key, factory, expiry) => factory())
            .ReturnsAsync(expectedDtos);

        // Act: gọi service lấy địa chỉ user
        var result = await _service.GetUserAddressAsync();

        // Assert: kiểm tra trả về đúng DTO
        Assert.Equal(expectedDtos, result);
        // Kiểm tra repo được gọi đúng 1 lần
        _mockUserRepository.Verify(x => x.GetByIdAsync(_userId, true), Times.Once);
        // Kiểm tra cacheService được gọi đúng 1 lần
        _mockRedisService.Verify(x => x.GetOrCreateAsync(
            It.IsAny<string>(),
            It.IsAny<Func<Task<List<UserAddressDto>>>>(),
            It.IsAny<TimeSpan?>()), Times.Once);
    }

    // Test tạo mới địa chỉ user khi user tồn tại
    [Fact]
    public async Task CreateUserAddressAsync_WhenUserExists_CreatesAddress()
    {
        // Arrange: tạo user giả lập
        var user = new User(
            phoneNumber: "84987654321",
            password: "hashedPassword123",
            userName: "testuser",
            fullName: "Test User",
            email: "test@example.com"
        );
        user.GetType().GetProperty("Id").SetValue(user, _userId);

        // Tạo DTO request tạo địa chỉ mới
        var requestDto = new UserAddressRequestDto
        {
            FullName = "Test User",
            Street = "123 Test St",
            Ward = "Test Ward",
            District = "Test District",
            Province = "Test Province",
            Country = "Test Country",
            ZipCode = "12345",
            PhoneNumber = "84987654321"
        };

        // Setup repo trả về user giả lập
        _mockUserRepository.Setup(x => x.GetByIdAsync(_userId, true))
            .ReturnsAsync(user);
        // Setup repo SaveChangesAsync trả về 1 (thành công) và callback gán Id cho địa chỉ mới
        _mockUserRepository.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1)
            .Callback(() =>
            {
                var address = user.Addresses.First();
                address.GetType().GetProperty("Id").SetValue(address, Guid.NewGuid());
            });

        // Act: gọi service tạo địa chỉ mới
        var result = await _service.CreateUserAddressAsync(requestDto);

        // Assert: kiểm tra Id địa chỉ tạo khác Guid.Empty
        Assert.NotEqual(Guid.Empty, result);
        // Kiểm tra repo lưu thay đổi được gọi 1 lần
        _mockUserRepository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        // Kiểm tra cache được xóa (invalidate) 1 lần
        _mockRedisService.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Once);
        // Kiểm tra user có đúng 1 địa chỉ
        Assert.Single(user.Addresses);
        var address = user.Addresses.First();
        // Kiểm tra các thông tin địa chỉ khớp với DTO request
        Assert.Equal(requestDto.FullName, address.FullName);
        Assert.Equal(requestDto.Street, address.Street);
        // Kiểm tra Id trả về trùng với Id địa chỉ trong user
        Assert.Equal(result, address.Id);
    }

    // Test cập nhật địa chỉ khi địa chỉ tồn tại
    [Fact]
    public async Task UpdateUserAddressAsync_WhenAddressExists_UpdatesAddress()
    {
        // Arrange: tạo user và gán Id
        var user = new User(
            phoneNumber: "84987654321",
            password: "hashedPassword123",
            userName: "testuser",
            fullName: "Test User",
            email: "test@example.com"
        );
        user.GetType().GetProperty("Id").SetValue(user, _userId);

        // Tạo địa chỉ cũ với thuộc tính cũ
        var addressId = Guid.NewGuid();
        var addressProps = new UserAddressProps
        {
            FullName = "Old Name",
            Street = "Old Street",
            PhoneNumber = "84987654321"
        };
        user.AddAddress(addressProps);
        var address = user.Addresses.First();
        address.GetType().GetProperty("Id").SetValue(address, addressId);

        // Tạo DTO cập nhật địa chỉ mới
        var requestDto = new UserAddressRequestDto
        {
            FullName = "New Name",
            Street = "New Street",
            Ward = "New Ward",
            District = "New District",
            Province = "New Province",
            Country = "New Country",
            ZipCode = "54321",
            PhoneNumber = "84987654321"
        };

        // Setup repo trả về user giả lập
        _mockUserRepository.Setup(x => x.GetByIdAsync(_userId, true))
            .ReturnsAsync(user);
        // Setup repo lưu thay đổi thành công
        _mockUserRepository.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act: gọi service cập nhật địa chỉ
        var result = await _service.UpdateUserAddressAsync(requestDto, addressId);

        // Assert: kiểm tra cập nhật thành công
        Assert.True(result);
        // Kiểm tra repo lưu thay đổi được gọi 1 lần
        _mockUserRepository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        // Kiểm tra cache bị xóa 1 lần
        _mockRedisService.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Once);
        // Kiểm tra địa chỉ được cập nhật đúng giá trị mới
        var updatedAddress = user.Addresses.First();
        Assert.Equal(requestDto.FullName, updatedAddress.FullName);
        Assert.Equal(requestDto.Street, updatedAddress.Street);
    }

    // Test đặt địa chỉ mặc định thành công khi địa chỉ tồn tại
    [Fact]
    public async Task SetDefaultUserAddressAsync_WhenAddressExists_SetsDefault()
    {
        // Arrange: tạo user và địa chỉ, gán Id
        var user = new User(
            phoneNumber: "84987654321",
            password: "hashedPassword123",
            userName: "testuser",
            fullName: "Test User",
            email: "test@example.com"
        );
        user.GetType().GetProperty("Id").SetValue(user, _userId);

        var addressId = Guid.NewGuid();
        var addressProps = new UserAddressProps
        {
            FullName = "Test User",
            Street = "Test Street",
            PhoneNumber = "84987654321"
        };
        user.AddAddress(addressProps);
        var address = user.Addresses.First();
        address.GetType().GetProperty("Id").SetValue(address, addressId);

        // Setup repo trả về user và lưu thay đổi thành công
        _mockUserRepository.Setup(x => x.GetByIdAsync(_userId, true))
            .ReturnsAsync(user);
        _mockUserRepository.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act: gọi service đặt địa chỉ mặc định
        var result = await _service.SetDefaultUserAddressAsync(addressId);

        // Assert: kiểm tra thành công
        Assert.True(result);
        // Kiểm tra repo lưu thay đổi 1 lần
        _mockUserRepository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        // Kiểm tra cache bị xóa 1 lần
        _mockRedisService.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Once);
        // Kiểm tra địa chỉ đầu tiên đã được đánh dấu là mặc định
        var defaultAddress = user.Addresses.First();
        Assert.True(defaultAddress.IsDefault);
    }

    // Test xóa địa chỉ thành công khi địa chỉ tồn tại
    [Fact]
    public async Task DeleteUserAddressAsync_WhenAddressExists_DeletesAddress()
    {
        // Arrange: tạo user và địa chỉ, gán Id
        var user = new User(
            phoneNumber: "84987654321",
            password: "hashedPassword123",
            userName: "testuser",
            fullName: "Test User",
            email: "test@example.com"
        );
        user.GetType().GetProperty("Id").SetValue(user, _userId);

        var addressId = Guid.NewGuid();
        var addressProps = new UserAddressProps
        {
            FullName = "Test User",
            Street = "Test Street",
            PhoneNumber = "84987654321"
        };
        user.AddAddress(addressProps);
        var address = user.Addresses.First();
        address.GetType().GetProperty("Id").SetValue(address, addressId);

        // Setup repo trả về user và lưu thay đổi thành công
        _mockUserRepository.Setup(x => x.GetByIdAsync(_userId, true))
            .ReturnsAsync(user);
        _mockUserRepository.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act: gọi service xóa địa chỉ
        var result = await _service.DeleteUserAddressAsync(addressId);

        // Assert: kiểm tra thành công xóa địa chỉ
        Assert.True(result);
        // Kiểm tra repo lưu thay đổi 1 lần
        _mockUserRepository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        // Kiểm tra cache bị xóa 1 lần
        _mockRedisService.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Once);
        // Kiểm tra user không còn địa chỉ nào
        Assert.Empty(user.Addresses);
    }

    // Test lấy địa chỉ theo Id khi địa chỉ tồn tại
    [Fact]
    public async Task GetByIdAsync_WhenAddressExists_ReturnsAddress()
    {
        // Arrange: tạo user và địa chỉ, gán Id
        var user = new User(
            phoneNumber: "84987654321",
            password: "hashedPassword123",
            userName: "testuser",
            fullName: "Test User",
            email: "test@example.com"
        );
        user.GetType().GetProperty("Id").SetValue(user, _userId);

        var addressId = Guid.NewGuid();
        var addressProps = new UserAddressProps
        {
            FullName = "Test User",
            Street = "Test Street",
            PhoneNumber = "84987654321"
        };
        user.AddAddress(addressProps);
        var address = user.Addresses.First();
        address.GetType().GetProperty("Id").SetValue(address, addressId);

        // Setup repo trả về user
        _mockUserRepository.Setup(x => x.GetByIdAsync(_userId, true))
            .ReturnsAsync(user);

        // Act: gọi lấy địa chỉ theo Id
        var result = await _service.GetByIdAsync(addressId);

        // Assert: kiểm tra địa chỉ không null và đúng Id
        Assert.NotNull(result);
        _mockUserRepository.Verify(x => x.GetByIdAsync(_userId, true), Times.Once);
        Assert.Equal(addressId, result.Id);
    }
}
