using Chat.Domain.Repositories;

namespace Chat.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    public Task BeginTransactionAsync()
    {
        throw new NotImplementedException();
    }

    public Task CommitAsync()
    {
        throw new NotImplementedException();
    }

    public Task RollbackAsync()
    {
        throw new NotImplementedException();
    }
}
