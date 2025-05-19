using HiveSpace.Application.Helpers;
using HiveSpace.Application.Interfaces;
using HiveSpace.Domain.AggergateModels.SkuAggregate;
using HiveSpace.Domain.Enums;
using HiveSpace.Domain.Exceptions;
using HiveSpace.Domain.Repositories;
using HiveSpace.Domain.Shared;

namespace HiveSpace.Application.Services
{
    public class SkuSerive : ISkuService
    {
        private readonly ISkuRepository _skuRepository;
        public SkuSerive(ISkuRepository skuRepository)
        {
            _skuRepository = skuRepository;
        }

        public async Task<List<Sku>> GetByFitlers(Dictionary<string, FilterItem> filters)
        {
            return await _skuRepository.GetByFilters(filters);
        }

        public async Task<bool> UpdateSkus(List<Sku> skus)
        {
            var filtersWithComparison = new Dictionary<string, FilterItem>
            {
                { "Id",
                new FilterItem
                {
                    Value=skus.Select(x=>x.Id),
                    Comparison=SqlOperator.In
                }
              }
            };
            List<Sku> foundSkus = await _skuRepository.GetByFilters(filtersWithComparison);

            foundSkus = foundSkus.Select(
                foundSku =>
                {
                    var skuUpdate = skus.Find(x => foundSku.Id == x.Id) ?? throw ExceptionHelper.NotFoundException(ApplicationErrorCode.NotFoundSku);
                    return skuUpdate;
                }
            ).ToList();

            return await _skuRepository.SaveChangesAsync() > 0;
        }
    }
}
