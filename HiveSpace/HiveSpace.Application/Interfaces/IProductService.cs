using HiveSpace.Application.Models.Dtos.Request.Product;
using HiveSpace.Application.Models.ViewModels;

namespace HiveSpace.Application.Interfaces
{
    public interface IProductService
    {
        Task<ProductDetailViewModel> GetProductDetailAsync(int productId);
        Task<List<ProductSearchViewModel>> GetProductSearchViewModelAsync(ProductSearchRequestDto param);
        Task<List<ProductHomeViewModel>> GetProductHomeViewModelAsync(int pageSize, int pageIndex);
        Task<List<ProductsByCategoryDto>> GetProductsByCategoryAsync(int categoryId);
    }
}
