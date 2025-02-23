using HiveSpace.Application.Models.ViewModels;

namespace HiveSpace.Application.Interfaces;

public interface ILocationService
{
    Task<List<LocationViewModel>> GetLocationAsync(int type, string? parentCode);
}
