using Microsoft.EntityFrameworkCore.Storage;

namespace HiveSpace.Application.Interfaces;

public interface ITransactionService
{
    Task InTransactionScopeAsync(Func<IDbContextTransaction, Task> action, bool performIdempotenceCheck = false);
    Task IdempotenceCheckAsync();
    Task OutOfOrderCheckAsync();
}
