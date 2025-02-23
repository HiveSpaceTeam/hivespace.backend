using HiveSpace.Application.Models.Dtos.Request.Product;
using HiveSpace.Application.Models.ViewModels;

namespace HiveSpace.Application.Queries;

public interface IQueryService
{
    Task<List<CategoryViewModel>> GetCategoryViewModelsAsync();
    Task<List<LocationViewModel>> GetLocationViewModelsAsync(int type, string? parentCode);
    Task<List<CartItemViewModel>> GetCartItemViewModelsAsync(Guid userId);
    Task<List<ProductSearchViewModel>> GetProductSearchViewModelAsync(ProductSearchRequestDto param);
    Task<List<ProductHomeViewModel>> GetProductHomeViewModelAsync(ProductHomeRequestDto param);
    Task<List<ProductsByCategoryDto>> GetProductSearchViewModelAsync(int categoryId);
    Task<List<ProductCategoryViewModel>> GetCategoryTree(int categoryId);
}
