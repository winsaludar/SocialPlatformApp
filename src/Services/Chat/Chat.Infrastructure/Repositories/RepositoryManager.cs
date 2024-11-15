﻿using Chat.Domain.Aggregates.MessageAggregate;
using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.Aggregates.UserAggregate;
using Chat.Domain.SeedWork;
using Microsoft.Extensions.Options;

namespace Chat.Infrastructure.Repositories;

public class RepositoryManager : IRepositoryManager
{
    private readonly Lazy<IServerRepository> _lazyServerRepository;
    private readonly Lazy<IUserRepository> _lazyUserRepository;
    private readonly Lazy<IMessageRepository> _lazyMessageRepository;

    public RepositoryManager(IOptions<ChatDbSettings> chatDbSettings)
    {
        _lazyServerRepository = new Lazy<IServerRepository>(() => new ServerRepository(chatDbSettings));
        _lazyUserRepository = new Lazy<IUserRepository>(() => new UserRepository(chatDbSettings));
        _lazyMessageRepository = new Lazy<IMessageRepository>(() => new MessageRepository(chatDbSettings));
    }

    public IServerRepository ServerRepository => _lazyServerRepository.Value;
    public IUserRepository UserRepository => _lazyUserRepository.Value;
    public IMessageRepository MessageRepository => _lazyMessageRepository.Value;
}
