using HiveSpace.Application.Helpers;
using HiveSpace.Application.Interfaces;
using HiveSpace.Application.Models.ViewModels;
using HiveSpace.Application.Queries;

namespace HiveSpace.Application.Services;

public class CategoryService(IQueryService queryService, ICacheService redisService) : ICategoryService
{
    private readonly IQueryService _queryService = queryService;
    private readonly ICacheService _redisService = redisService;


    public async Task<List<CategoryViewModel>> GetCategoryAsync()
    {
        return await _redisService.GetOrCreateAsync(CacheKeys.Categories, _queryService.GetCategoryViewModelsAsync);
    }
}
