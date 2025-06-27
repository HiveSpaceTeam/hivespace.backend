using AutoMapper;
using HiveSpace.Application.Models.Dtos.Request.Product;
using HiveSpace.Application.Models.ViewModels;
using HiveSpace.Application.Queries;
using HiveSpace.Application.Services;
using HiveSpace.Common.Exceptions;
using HiveSpace.Domain.AggergateModels.ProductAggregate;
using HiveSpace.Domain.Enums;
using HiveSpace.Domain.Repositories;
using Moq;

namespace HiveSpace.UnitTests.Services
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _productRepoMock;
        private readonly Mock<IQueryService> _queryServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ProductService _productService;

        public ProductServiceTests()
        {
            _productRepoMock = new Mock<IProductRepository>();
            _queryServiceMock = new Mock<IQueryService>();
            _mapperMock = new Mock<IMapper>();

            _productService = new ProductService(
                _productRepoMock.Object,
                _queryServiceMock.Object,
                _mapperMock.Object
            );
        }

        [Fact]
        public async Task GetProductDetailAsync_ReturnsProductDetail_WhenFound()
        {
            // Arrange
            int productId = 1;
            var product = new Product(
                "T-Shirt",
                "Blue T-Shirt",
                ProductStatus.Available,
                new List<ProductCategory> { },
                new List<ProductAttributeValue> { },
                new List<ProductVariant>(),
                new List<ProductImage>()
            );

            var mappedViewModel = new ProductDetailViewModel();

            var categoryTree = new List<ProductCategoryViewModel> { new ProductCategoryViewModel { CategoryId = 2, CategoryName = "Shirts" } };

            _productRepoMock.Setup(repo => repo.GetByIdAsync(productId, true)).ReturnsAsync(product);
            _mapperMock.Setup(m => m.Map<ProductDetailViewModel>(product)).Returns(mappedViewModel);
            _queryServiceMock
                .Setup(q => q.GetCategoryTree(2))
                .ReturnsAsync(categoryTree);

            // Act
            var result = await _productService.GetProductDetailAsync(productId);

            // Assert
            Assert.Equal(mappedViewModel, result);
        }

        [Fact]
        public async Task GetProductSearchViewModelAsync_ReturnsCorrectResult()
        {
            var request = new ProductSearchRequestDto();
            var expected = new List<ProductSearchViewModel> { new ProductSearchViewModel() };

            _queryServiceMock.Setup(q => q.GetProductSearchViewModelAsync(request)).ReturnsAsync(expected);

            var result = await _productService.GetProductSearchViewModelAsync(request);

            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task GetProductHomeViewModelAsync_ReturnsCorrectResult()
        {
            var expected = new List<ProductHomeViewModel> { new ProductHomeViewModel() };

            _queryServiceMock.Setup(q => q.GetProductHomeViewModelAsync(10, 1)).ReturnsAsync(expected);

            var result = await _productService.GetProductHomeViewModelAsync(10, 1);

            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task GetProductsByCategoryAsync_ReturnsCorrectResult()
        {
            int categoryId = 3;
            var expected = new List<ProductsByCategoryDto> { new ProductsByCategoryDto() };

            _queryServiceMock.Setup(q => q.GetProductSearchViewModelAsync(categoryId)).ReturnsAsync(expected);

            var result = await _productService.GetProductsByCategoryAsync(categoryId);

            Assert.Equal(expected, result);
        }
    }

}
