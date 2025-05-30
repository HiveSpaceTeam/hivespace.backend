using HiveSpace.Application.Interfaces;                // Interface các service, repository...
using HiveSpace.Application.Models.Dtos.Request.CartItem;     // DTO request thao tác CartItem
using HiveSpace.Application.Models.Dtos.Request.ShoppingCart; // DTO request thao tác ShoppingCart
using HiveSpace.Application.Models.Dtos.Request.UserAddress;  // DTO request địa chỉ người dùng
using HiveSpace.Application.Models.ViewModels;                 // ViewModel trả về client
using HiveSpace.Application.Queries;                            // Các query service
using HiveSpace.Application.Services;                           // Service chính cần test
using HiveSpace.Common.Interface;                               // Interface dùng chung
using HiveSpace.Domain.AggergateModels.ShoppingCartAggregate;   // Aggregate ShoppingCart
using HiveSpace.Domain.AggergateModels.SkuAggregate;            // Aggregate Sku
using HiveSpace.Domain.Repositories;                            // Repository interfaces
using Moq;                                                      // Thư viện mock cho unit test

namespace HiveSpace.Test.Services;

public class ShoppingCartServiceTests
{
    // Tạo các mock để giả lập các dependencies của ShoppingCartService
    private readonly Mock<IQueryService> _mockQueryService;
    private readonly Mock<IUserContext> _mockUserContext;
    private readonly Mock<IUserAddressService> _mockUserAddressService;
    private readonly Mock<IShoppingCartRepository> _mockShoppingCartRepository;
    private readonly Mock<ISkuRepository> _mockSkuRepository;
    private readonly ShoppingCartService _service;     // Đối tượng service thật cần test
    private readonly Guid _userId = Guid.NewGuid();    // ID người dùng giả lập

    // Constructor khởi tạo mock và đối tượng service để test
    public ShoppingCartServiceTests()
    {
        _mockQueryService = new Mock<IQueryService>();
        _mockUserContext = new Mock<IUserContext>();
        _mockUserAddressService = new Mock<IUserAddressService>();
        _mockShoppingCartRepository = new Mock<IShoppingCartRepository>();
        _mockSkuRepository = new Mock<ISkuRepository>();

        // Setup UserId của userContext trả về _userId giả lập
        _mockUserContext.Setup(x => x.UserId).Returns(_userId);

        // Tạo đối tượng ShoppingCartService thật sử dụng các mock ở trên
        _service = new ShoppingCartService(
            _mockUserContext.Object,
            _mockQueryService.Object,
            _mockShoppingCartRepository.Object,
            _mockSkuRepository.Object,
            _mockUserAddressService.Object
        );
    }

    // Test: Khi có giỏ hàng cho user thì GetShoppingCartByUserIdAsync trả về đúng giỏ hàng
    [Fact]
    public async Task GetShoppingCartByUserIdAsync_WhenCartExists_ReturnsCart()
    {
        // Arrange: chuẩn bị dữ liệu giỏ hàng giả lập
        var cartId = Guid.NewGuid();
        var cart = new ShoppingCart(_userId);                     // Tạo giỏ hàng với userId
        cart.GetType().GetProperty("Id").SetValue(cart, cartId);  // Set Id giỏ hàng (dùng reflection vì Id có thể private)

        var cartItems = new List<CartItemViewModel>();            // Danh sách sản phẩm trong giỏ
        var expectedResult = new CartViewModel                     // Kết quả mong muốn
        {
            Id = cartId,
            Items = cartItems
        };

        // Setup repository trả về giỏ hàng giả lập
        _mockShoppingCartRepository.Setup(x => x.GetShoppingCartByUserIdAsync(_userId))
            .ReturnsAsync(cart);
        // Setup queryService trả về danh sách sản phẩm giả lập
        _mockQueryService.Setup(x => x.GetCartItemViewModelsAsync(_userId))
            .ReturnsAsync(cartItems);

        // Act: gọi hàm service cần test
        var result = await _service.GetShoppingCartByUserIdAsync();

        // Assert: kiểm tra kết quả trả về đúng như mong đợi
        Assert.Equal(expectedResult.Id, result.Id);
        Assert.Equal(expectedResult.Items, result.Items);
        // Kiểm tra repository và queryService được gọi đúng 1 lần
        _mockShoppingCartRepository.Verify(x => x.GetShoppingCartByUserIdAsync(_userId), Times.Once);
        _mockQueryService.Verify(x => x.GetCartItemViewModelsAsync(_userId), Times.Once);
    }

    // Test: Khi cập nhật số lượng item hợp lệ thì item được cập nhật
    [Fact]
    public async Task UpdateCartItem_WhenValidQuantity_UpdatesItem()
    {
        // Arrange: tạo giỏ hàng và gán Id
        var cartId = Guid.NewGuid();
        var cart = new ShoppingCart(_userId);
        cart.GetType().GetProperty("Id").SetValue(cart, cartId);

        // Tạo SKU giả lập (hàng hóa)
        var sku = new Sku(
            skuNo: "TEST-SKU",
            productId: 1,
            skuVariants: new List<SkuVariant>(),
            quantity: 5,
            inActive: false,
            amount: 100,
            currency: Domain.Enums.Currency.VND
        );

        // Setup repository SKU trả về SKU giả lập
        _mockSkuRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<bool>()))
            .ReturnsAsync(sku);

        // Setup repository trả về giỏ hàng và lưu thay đổi thành công
        _mockShoppingCartRepository.Setup(x => x.GetShoppingCartByUserIdAsync(_userId))
            .ReturnsAsync(cart);
        _mockShoppingCartRepository.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Thêm 1 item vào giỏ bằng phương thức add item
        var addRequest = new AddItemToCartRequestDto
        {
            Quantity = 1,
            SkuId = 1,
            IsSelected = true
        };

        await _service.AddItemToCartAsync(addRequest);

        // Lấy item thực tế trong giỏ sau khi add
        var cartItem = cart.Items.First();

        // Tạo DTO cập nhật số lượng item
        var updateRequest = new UpdateCartItemRequestDto
        {
            CartId = cartId,
            Id = cartItem.SkuId,
            Quantity = 2
        };

        // Setup repository trả về giỏ hàng khi lấy theo Id
        _mockShoppingCartRepository.Setup(x => x.GetByIdAsync(cartId, true))
            .ReturnsAsync(cart);

        // Act: gọi hàm update item
        var result = await _service.UpdateCartItem(updateRequest);

        // Assert: kiểm tra kết quả update thành công
        Assert.True(result);
        // Kiểm tra SaveChangesAsync được gọi 2 lần (1 lần add, 1 lần update)
        _mockShoppingCartRepository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
        // Kiểm tra số lượng của item đã được cập nhật đúng
        var updatedItem = cart.Items.First();
        Assert.Equal(updateRequest.Quantity, updatedItem.Quantity);
    }

    // Test: Khi xóa nhiều item có tồn tại trong giỏ thì xóa thành công
    [Fact]
    public async Task DeleteCartItems_WhenItemsExist_DeletesItems()
    {
        // Arrange: tạo giỏ hàng mới
        var cart = new ShoppingCart(_userId);

        // Tạo SKU giả lập
        var sku = new Sku(
            skuNo: "TEST-SKU",
            productId: 1,
            skuVariants: new List<SkuVariant>(),
            quantity: 5,
            inActive: false,
            amount: 100,
            currency: Domain.Enums.Currency.VND
        );

        // Setup repository SKU trả về SKU giả lập
        _mockSkuRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<bool>()))
            .ReturnsAsync(sku);

        // Setup repository trả về giỏ hàng và lưu thành công
        _mockShoppingCartRepository.Setup(x => x.GetShoppingCartByUserIdAsync(_userId))
            .ReturnsAsync(cart);
        _mockShoppingCartRepository.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Thêm 1 item vào giỏ
        var addRequest1 = new AddItemToCartRequestDto
        {
            Quantity = 1,
            SkuId = 1,
            IsSelected = true
        };

        await _service.AddItemToCartAsync(addRequest1);

        // Lấy danh sách Id các item trong giỏ
        var itemIds = cart.Items.Select(x => x.Id).ToList();

        // Act: gọi xóa nhiều item
        var result = await _service.DeleteCartItems(itemIds);

        // Assert: kiểm tra xóa thành công, giỏ hàng không còn item
        Assert.True(result);
        _mockShoppingCartRepository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
        Assert.Empty(cart.Items);
    }

    // Test: Cập nhật lựa chọn nhiều item trong giỏ (IsSelected) thành công
    [Fact]
    public async Task UpdateMultiSelection_WhenCartExists_UpdatesSelection()
    {
        // Arrange: tạo giỏ hàng, set Id
        var cartId = Guid.NewGuid();
        var cart = new ShoppingCart(_userId);
        cart.GetType().GetProperty("Id").SetValue(cart, cartId);

        // Tạo SKU giả lập
        var sku = new Sku(
            skuNo: "TEST-SKU",
            productId: 1,
            skuVariants: new List<SkuVariant>(),
            quantity: 5,
            inActive: false,
            amount: 100,
            currency: Domain.Enums.Currency.VND
        );

        // Setup trả về SKU giả lập
        _mockSkuRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<bool>()))
            .ReturnsAsync(sku);

        // Thêm 2 item vào giỏ
        var addRequest1 = new AddItemToCartRequestDto
        {
            Quantity = 1,
            SkuId = 1,
            IsSelected = true
        };

        var addRequest2 = new AddItemToCartRequestDto
        {
            Quantity = 1,
            SkuId = 2,
            IsSelected = true
        };

        // Setup repository trả về giỏ hàng và lưu thành công
        _mockShoppingCartRepository.Setup(x => x.GetShoppingCartByUserIdAsync(_userId))
            .ReturnsAsync(cart);
        _mockShoppingCartRepository.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _service.AddItemToCartAsync(addRequest1);
        await _service.AddItemToCartAsync(addRequest2);

        // DTO cập nhật trạng thái IsSelected nhiều item
        var updateRequest = new UpdateMultiCartItemSelectionDto
        {
            CartId = cartId,
            SkuIds = new List<int> { 1, 2 },
            IsSelected = true
        };

        // Setup repository trả về giỏ hàng khi lấy theo Id
        _mockShoppingCartRepository.Setup(x => x.GetByIdAsync(cartId, true))
            .ReturnsAsync(cart);

        // Act: gọi cập nhật lựa chọn nhiều item
        var result = await _service.UpdateMultiSelection(updateRequest);

        // Assert: kiểm tra cập nhật thành công, tất cả item đều được chọn (IsSelected = true)
        Assert.True(result);
        _mockShoppingCartRepository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(3));
        Assert.All(cart.Items, item => Assert.True(item.IsSelected));
    }

    // Test: Xóa một item trong giỏ thành công
    [Fact]
    public async Task DeleteCartItem_WhenItemExists_DeletesItem()
    {
        // Arrange: tạo giỏ hàng
        var cart = new ShoppingCart(_userId);

        // Tạo SKU giả lập
        var sku = new Sku(
            skuNo: "TEST-SKU",
            productId: 1,
            skuVariants: new List<SkuVariant>(),
            quantity: 5,
            inActive: false,
            amount: 100,
            currency: Domain.Enums.Currency.VND
        );

        // Setup trả về SKU giả lập
        _mockSkuRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<bool>()))
            .ReturnsAsync(sku);

        // Thêm 1 item vào giỏ
        var addRequest = new AddItemToCartRequestDto
        {
            Quantity = 1,
            SkuId = 1,
            IsSelected = true
        };

        // Setup trả về giỏ hàng và lưu thành công
        _mockShoppingCartRepository.Setup(x => x.GetShoppingCartByUserIdAsync(_userId))
            .ReturnsAsync(cart);
        _mockShoppingCartRepository.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        await _service.AddItemToCartAsync(addRequest);

        // Lấy Id item trong giỏ
        var cartItemId = cart.Items.First().Id;

        // Act: gọi xóa 1 item
        var result = await _service.DeleteCartItem(cartItemId);

        // Assert: kiểm tra xóa thành công, giỏ hàng rỗng
        Assert.True(result);
        _mockShoppingCartRepository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
        Assert.Empty(cart.Items);
    }

    // Test: Thêm item vào giỏ thành công với số lượng hợp lệ
    [Fact]
    public async Task AddItemToCartAsync_WhenValidQuantity_AddsItem()
    {
        // Arrange: tạo giỏ hàng và DTO thêm item
        var cart = new ShoppingCart(_userId);
        var addRequest = new AddItemToCartRequestDto
        {
            Quantity = 2,
            SkuId = 1,
            IsSelected = true
        };

        // Tạo SKU giả lập
        var sku = new Sku(
            skuNo: "TEST-SKU",
            productId: 1,
            skuVariants: new List<SkuVariant>(),
            quantity: 5,
            inActive: false,
            amount: 100,
            currency: Domain.Enums.Currency.VND
        );

        // Setup trả về giỏ hàng, SKU và lưu thành công
        _mockShoppingCartRepository.Setup(x => x.GetShoppingCartByUserIdAsync(_userId))
            .ReturnsAsync(cart);
        _mockSkuRepository.Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<bool>()))
            .ReturnsAsync(sku);
        _mockShoppingCartRepository.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act: gọi thêm item vào giỏ
        var result = await _service.AddItemToCartAsync(addRequest);

        // Assert: kiểm tra thêm thành công, giỏ hàng có 1 item với thông tin đúng
        Assert.True(result);
        _mockShoppingCartRepository.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        Assert.Single(cart.Items);
        var cartItem = cart.Items.First();
        Assert.Equal(addRequest.SkuId, cartItem.SkuId);
        Assert.Equal(addRequest.Quantity, cartItem.Quantity);
        Assert.Equal(addRequest.IsSelected, cartItem.IsSelected);
    }

    // Test: Lấy thông tin checkout khi có dữ liệu trả về thành công
    [Fact]
    public async Task GetCheckOutAsync_WhenDataExists_ReturnsCheckoutData()
    {
        // Arrange: chuẩn bị dữ liệu địa chỉ và sản phẩm chọn mua giả lập
        var addresses = new List<UserAddressDto>
        {
            new() { IsDefault = true }   // Có 1 địa chỉ mặc định
        };
        var cartItems = new List<CartItemViewModel>
        {
            new() { IsSelected = true }  // Có 1 sản phẩm được chọn mua
        };

        // Setup trả về danh sách địa chỉ, giỏ hàng và danh sách sản phẩm trong giỏ
        _mockUserAddressService.Setup(x => x.GetUserAddressAsync())
            .ReturnsAsync(addresses);
        _mockShoppingCartRepository.Setup(x => x.GetShoppingCartByUserIdAsync(_userId))
            .ReturnsAsync(new ShoppingCart(_userId));
        _mockQueryService.Setup(x => x.GetCartItemViewModelsAsync(_userId))
            .ReturnsAsync(cartItems);

        // Act: gọi lấy dữ liệu checkout
        var result = await _service.GetCheckOutAsync();

        // Assert: kiểm tra dữ liệu trả về hợp lệ
        Assert.NotNull(result);
        Assert.NotNull(result.Address);
        Assert.NotEmpty(result.Products);
        _mockUserAddressService.Verify(x => x.GetUserAddressAsync(), Times.Once);
        _mockShoppingCartRepository.Verify(x => x.GetShoppingCartByUserIdAsync(_userId), Times.Once);
        _mockQueryService.Verify(x => x.GetCartItemViewModelsAsync(_userId), Times.Once);
    }
}
