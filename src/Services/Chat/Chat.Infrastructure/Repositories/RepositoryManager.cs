using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.SeedWork;
using Microsoft.Extensions.Options;

namespace Chat.Infrastructure.Repositories;

public class RepositoryManager : IRepositoryManager
{
    private readonly Lazy<IServerRepository> _lazyServerRepository;

    public RepositoryManager(IOptions<ChatDbSettings> chatDbSettings)
    {
        _lazyServerRepository = new Lazy<IServerRepository>(() => new ServerRepository(chatDbSettings));
    }

    public IServerRepository ServerRepository => _lazyServerRepository.Value;
}
