using HiveSpace.Domain.AggergateModels.SkuAggregate;
using HiveSpace.Domain.Enums;
using HiveSpace.Domain.Shared;

namespace HiveSpace.Application.Interfaces
{
    public interface ISkuService
    {
        Task<List<Sku>> GetByFitlers(Dictionary<string,FilterItem> filters);
        Task<bool> UpdateSkus(List<Sku> sku);
    }
}
