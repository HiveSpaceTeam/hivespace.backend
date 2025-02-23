using HiveSpace.Application.Models.ViewModels;

namespace HiveSpace.Application.Interfaces;

public interface ICategoryService
{
    Task<List<CategoryViewModel>> GetCategoryAsync();
}
