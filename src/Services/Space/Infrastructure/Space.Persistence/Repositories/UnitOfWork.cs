using Microsoft.EntityFrameworkCore.Storage;
using Space.Domain.Repositories;

namespace Space.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly SpaceDbContext _dbContext;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(SpaceDbContext dbContext) => _dbContext = dbContext;

    public async Task BeginTransactionAsync()
    {
        _transaction = await _dbContext.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        if (_transaction == null)
            throw new ArgumentException("IDbContextTransaction is null");

        await _dbContext.SaveChangesAsync();
        await _transaction.CommitAsync();
        await _transaction.DisposeAsync();
    }

    public async Task RollbackAsync()
    {
        if (_transaction == null)
            throw new ArgumentException("IDbContextTransaction is null");

        await _transaction.RollbackAsync();
    }
}
