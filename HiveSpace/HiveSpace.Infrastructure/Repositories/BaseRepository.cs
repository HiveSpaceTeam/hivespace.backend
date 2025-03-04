using Microsoft.EntityFrameworkCore;
using HiveSpace.Common.Utilities;
using HiveSpace.Domain.Enums;
using HiveSpace.Domain.Seedwork;
using HiveSpace.Domain.SeedWork;
using HiveSpace.Domain.Shared;
using System.Linq.Expressions;

namespace HiveSpace.Infrastructure.Repositories;
public abstract class BaseRepository<TEntity, TKey>(HiveSpaceDbContext context) : IRepository<TEntity>
    where TEntity : AggregateRoot<TKey>
    where TKey : struct, IEquatable<TKey>
{
    protected readonly HiveSpaceDbContext _context = context;

    public void Add(TEntity entity)
    {
        _context.Set<TEntity>().AddAsync(entity);

    }

    public async Task<List<TEntity>> GetAllAsync()
    {
        return await _context.Set<TEntity>().AsNoTracking().ToListAsync();
    }

    protected virtual IQueryable<TEntity> ApplyIncludeDetail(IQueryable<TEntity> query)
    {
        return query; // Default implementation returns query without includes
    }

    public async Task<TEntity?> GetByIdAsync(object id, bool includeDetail = false)
    {
        IQueryable<TEntity> query = _context.Set<TEntity>();
        if (includeDetail)
        {
            query = ApplyIncludeDetail(query);
        }

        return await query.FirstOrDefaultAsync(entity => EF.Property<object>(entity, "Id") == id);
    }



    private IQueryable<TEntity> GetQueryByFilters(Dictionary<string, FilterItem> filters)
    {
        var parameter = Expression.Parameter(typeof(TEntity), "x");
        IQueryable<TEntity> query = _context.Set<TEntity>();

        if (filters == null)
        {
            return query;
        }

        foreach (var filter in filters)
        {
            Expression property = Expression.Property(parameter, filter.Key);
            if (property.Type.IsEnum)
            {
                property = Expression.Convert(property, typeof(Int64));
            }
            var constant = Expression.Constant(CommonFunction.ConvertToActualType(filter.Value.Value));

            Expression comparisonExpression = null;

            switch (filter.Value.Comparison)
            {
                case SqlOperator.Equal:
                    comparisonExpression = Expression.Equal(property, constant);
                    break;
                case SqlOperator.Notequal:
                    comparisonExpression = Expression.NotEqual(property, constant);
                    break;
                case SqlOperator.Greaterthan:
                    comparisonExpression = Expression.GreaterThan(property, constant);
                    break;
                case SqlOperator.Lessthan:
                    comparisonExpression = Expression.LessThan(property, constant);
                    break;
                case SqlOperator.In:
                    comparisonExpression = Expression.Call(
                        typeof(Enumerable),
                        "Contains",
                        new Type[] { property.Type },
                        constant,
                        property
                    );

                    break;
                default:
                    throw new ArgumentException($"Unsupported comparison: {filter.Value.Comparison}");
            }

            var lambda = Expression.Lambda<Func<TEntity, bool>>(comparisonExpression, parameter);
            query = query.Where(lambda);
        }

        return query;
    }


    public async Task<List<TEntity>> GetByFilters(Dictionary<string, FilterItem> filters)
    {
        IQueryable<TEntity> query = GetQueryByFilters(filters);
        return await query.ToListAsync();
    }

    public async Task<List<TEntity>> GetPaging(int pageNumber, int pageSize, Dictionary<string, FilterItem> filters, bool includeDetail)
    {
        IQueryable<TEntity> query = _context.Set<TEntity>();

        //TODO: check param 

        if (filters != null)
        {
            query = GetQueryByFilters(filters);
        }

        if (includeDetail)
        {
            query = ApplyIncludeDetail(query);
        }

        query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

        return await query.ToListAsync();
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
