using HiveSpace.Application.Models.Dtos.Request.Product;
using HiveSpace.Application.Models.ViewModels;

namespace HiveSpace.Application.Interfaces
{
    public interface IProductService
    {
        Task<ProductDetailViewModel> GetProductDetailAsync(int productId);
        Task<List<ProductSearchViewModel>> GetProductSearchViewModelAsync(ProductSearchRequestDto param);
        Task<List<ProductHomeViewModel>> GetProductHomeViewModelAsync(ProductHomeRequestDto param);
        Task<List<ProductsByCategoryDto>> GetProductsByCategoryAsync(int categoryId);
    }
}
