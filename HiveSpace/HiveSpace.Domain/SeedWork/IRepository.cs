using HiveSpace.Domain.Enums;
using HiveSpace.Domain.SeedWork;
using HiveSpace.Domain.Shared;

namespace HiveSpace.Domain.Seedwork;

public interface IRepository<TEntity> where TEntity : IAggregateRoot
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    void Add(TEntity entity);
    Task<List<TEntity>> GetAllAsync();
    Task<TEntity?> GetByIdAsync(object id, bool includeDetail = false);
    Task<List<TEntity>> GetPaging(int pageNumber, int pageSize, Dictionary<string, FilterItem> filters, bool includeDetail);
    Task<List<TEntity>> GetByFilters(Dictionary<string, FilterItem> filters);
    void UpdateWithConcurrency<T>(T entity, DateTimeOffset originalDateTimeUpdated, DateTimeOffset newDateTimeUpdated) where T : TEntity, IAuditable;
    void UpdateWithConcurrency<T>(T entity, DateTimeOffset originalDateTimeUpdated) where T : TEntity, IAuditable;
}
