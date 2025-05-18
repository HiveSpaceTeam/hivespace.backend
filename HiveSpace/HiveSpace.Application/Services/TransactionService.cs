using HiveSpace.Application.Interfaces;
using HiveSpace.Common.Exceptions;
using HiveSpace.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace HiveSpace.Application.Services;

public class TransactionService(HiveSpaceDbContext context) : ITransactionService
{
    private readonly HiveSpaceDbContext _dbContext = context;

    public Task IdempotenceCheckAsync()
    {
        throw new NotImplementedException();
    }

    public async Task InTransactionScopeAsync(Func<IDbContextTransaction, Task> action, bool performIdempotenceCheck)
    {
        var executionStrategy = _dbContext.Database.CreateExecutionStrategy();
        await executionStrategy.ExecuteAsync(async () =>
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            if (performIdempotenceCheck)
            {
                await IdempotenceCheckAsync();
            }

            try
            {
                await action?.Invoke(transaction)!;
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await transaction.RollbackAsync();
                _dbContext.ChangeTracker.Clear();
                throw new ConcurrencyException([new () { Code = CommonErrorCode.ConcurrencyException }], ex);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
            
        });
    }

    public Task OutOfOrderCheckAsync()
    {
        throw new NotImplementedException();
    }
}
