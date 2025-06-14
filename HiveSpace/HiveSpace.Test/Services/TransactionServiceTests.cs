//using HiveSpace.Application.Services;
//using HiveSpace.Infrastructure;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Infrastructure;
//using Microsoft.EntityFrameworkCore.Storage;
//using Moq;
//using System.Data;

//namespace HiveSpace.Test.Services;

//public class TransactionServiceTests
//{
//    private readonly Mock<HiveSpaceDbContext> _mockDbContext;
//    private readonly Mock<DatabaseFacade> _mockDbFacade;
//    private readonly Mock<IDbContextTransaction> _mockTransaction;
//    private readonly TransactionService _transactionService;

//    public TransactionServiceTests()
//    {
//        _mockDbContext = new Mock<HiveSpaceDbContext>();
//        _mockDbFacade = new Mock<DatabaseFacade>(_mockDbContext.Object);
//        _mockTransaction = new Mock<IDbContextTransaction>();

//        _mockDbContext
//            .Setup(x => x.Database)
//            .Returns(_mockDbFacade.Object);

//        _mockDbContext
//            .Setup(x => x.Database.CreateExecutionStrategy())
//            .Returns(new TestExecutionStrategy());

//        _mockDbContext
//            .Setup(x => x.Database.BeginTransactionAsync(It.IsAny<IsolationLevel>(), default))
//            .ReturnsAsync(_mockTransaction.Object);

//        _transactionService = new TransactionService(_mockDbContext.Object);
//    }

//    [Fact]
//    public async Task InTransactionScopeAsync_ShouldCommitTransaction_WhenActionSucceeds()
//    {
//        // Arrange
//        var actionExecuted = false;

//        // Act
//        await _transactionService.InTransactionScopeAsync(async transaction =>
//        {
//            actionExecuted = true;
//        }, performIdempotenceCheck: false);

//        // Assert
//        Assert.True(actionExecuted);
//        _mockTransaction.Verify(x => x.CommitAsync(default), Times.Once);
//        _mockTransaction.Verify(x => x.RollbackAsync(default), Times.Never);
//        _mockDbContext.Verify(x => x.SaveChangesAsync(default), Times.Once);
//    }

//    [Fact]
//    public async Task InTransactionScopeAsync_ShouldRollbackTransaction_WhenActionThrowsException()
//    {
//        // Arrange
//        var actionExecuted = false;

//        // Act & Assert
//        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
//        {
//            await _transactionService.InTransactionScopeAsync(async transaction =>
//            {
//                actionExecuted = true;
//                throw new InvalidOperationException("Test exception");
//            }, performIdempotenceCheck: false);
//        });

//        // Assert
//        Assert.True(actionExecuted);
//        _mockTransaction.Verify(x => x.CommitAsync(default), Times.Never);
//        _mockTransaction.Verify(x => x.RollbackAsync(default), Times.Once);
//        _mockDbContext.Verify(x => x.SaveChangesAsync(default), Times.Never);
//    }

//    [Fact]
//    public async Task InTransactionScopeAsync_ShouldCallIdempotenceCheck_WhenPerformIdempotenceCheckIsTrue()
//    {
//        // Arrange
//        var idempotenceCheckCalled = false;
//        var transactionService = new Mock<TransactionService>(_mockDbContext.Object)
//        {
//            CallBase = true
//        };

//        transactionService
//            .Setup(x => x.IdempotenceCheckAsync())
//            .Callback(() => idempotenceCheckCalled = true)
//            .Returns(Task.CompletedTask);

//        // Act
//        await transactionService.Object.InTransactionScopeAsync(async transaction => { }, performIdempotenceCheck: true);

//        // Assert
//        Assert.True(idempotenceCheckCalled);
//        transactionService.Verify(x => x.IdempotenceCheckAsync(), Times.Once);
//    }
//}

//// Custom execution strategy for testing
//public class TestExecutionStrategy : IExecutionStrategy
//{
//    public bool RetriesOnFailure => throw new NotImplementedException();

//    public void Execute(Action operation) => operation();

//    public TResult Execute<TResult>(Func<TResult> operation) => operation();

//    public TResult Execute<TState, TResult>(TState state, Func<DbContext, TState, TResult> operation, Func<DbContext, TState, ExecutionResult<TResult>>? verifySucceeded)
//    {
//        throw new NotImplementedException();
//    }

//    public Task ExecuteAsync(Func<Task> operation, CancellationToken cancellationToken = default) => operation();

//    public Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> operation, CancellationToken cancellationToken = default) => operation();

//    public Task<TResult> ExecuteAsync<TState, TResult>(TState state, Func<DbContext, TState, CancellationToken, Task<TResult>> operation, Func<DbContext, TState, CancellationToken, Task<ExecutionResult<TResult>>>? verifySucceeded, CancellationToken cancellationToken = default)
//    {
//        throw new NotImplementedException();
//    }
//}
