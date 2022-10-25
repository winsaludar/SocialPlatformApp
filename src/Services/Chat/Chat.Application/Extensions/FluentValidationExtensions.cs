using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.Exceptions;
using Chat.Domain.SeedWork;
using FluentValidation;

namespace Chat.Application.Extensions;

public static class FluentValidationExtensions
{
    public static IRuleBuilderOptions<T, Guid> MustBeExistingServer<T>(this IRuleBuilder<T, Guid> ruleBuilder, IRepositoryManager repositoryManager)
    {
        return ruleBuilder.MustAsync(async (serverId, cancellationToken) =>
        {
            var result = await repositoryManager.ServerRepository.GetByIdAsync(serverId);
            if (result is null)
                throw new ServerNotFoundException(serverId.ToString());

            return true;
        });
    }

    public static IRuleBuilderOptions<T, string> MustNotBeExistingServerName<T>(this IRuleBuilder<T, string> ruleBuilder, IRepositoryManager repositoryManager)
    {
        return ruleBuilder.MustAsync(async (name, cancellationToken) =>
        {
            var result = await repositoryManager.ServerRepository.GetByNameAsync(name);
            if (result is not null)
                throw new ServerNameAlreadyExistException(name);

            return true;
        });
    }

    public static IRuleBuilderOptions<T, Server> MustBeExistingServer<T>(this IRuleBuilder<T, Server> ruleBuilder, IRepositoryManager repositoryManager)
    {
        return ruleBuilder.MustAsync(async (server, cancellationToken) =>
        {
            var result = await repositoryManager.ServerRepository.GetByIdAsync(server.Id);
            if (result is null)
                throw new ServerNotFoundException(server.Id.ToString());

            return true;
        });
    }

    public static IRuleBuilderOptions<T, Tuple<Guid, Guid>> MustBeExistingChannel<T>(this IRuleBuilder<T, Tuple<Guid, Guid>> ruleBuilder, IRepositoryManager repositoryManager)
    {
        return ruleBuilder.MustAsync(async (props, cancellationToken) =>
        {
            (Guid serverId, Guid channelId) = props;

            var server = await repositoryManager.ServerRepository.GetByIdAsync(serverId);
            if (server is null)
                throw new ServerNotFoundException(serverId.ToString());

            if (!server.Channels.Any(x => x.Id == channelId))
                throw new ChannelNotFoundException(channelId.ToString());

            return true;
        });
    }

    public static IRuleBuilderOptions<T, Tuple<Server, string>> MustNotBeExistingChannelName<T>(this IRuleBuilder<T, Tuple<Server, string>> ruleBuilder, IRepositoryManager repositoryManager)
    {
        return ruleBuilder.MustAsync(async (props, cancellationToken) =>
        {
            (Server targetServer, string channelName) = props;

            var server = await repositoryManager.ServerRepository.GetByIdAsync(targetServer.Id);
            if (server is null)
                throw new ServerNotFoundException(targetServer.Id.ToString());

            if (server.Channels.Any(x => x.Name.ToLower() == channelName.ToLower()))
                throw new ChannelNameAlreadyExistException(channelName);

            return true;
        });
    }

    public static IRuleBuilderOptions<T, Tuple<Server, Guid, string>> MustNotBeExistingChannelName<T>(this IRuleBuilder<T, Tuple<Server, Guid, string>> ruleBuilder, IRepositoryManager repositoryManager)
    {
        return ruleBuilder.MustAsync(async (props, cancellationToken) =>
        {
            (Server targetServer, Guid targetChannelId, string channelName) = props;

            var server = await repositoryManager.ServerRepository.GetByIdAsync(targetServer.Id);
            if (server is null)
                throw new ServerNotFoundException(targetServer.Id.ToString());

            var channel = server.Channels.FirstOrDefault(x => x.Name.ToLower() == channelName.ToLower());
            if (channel is not null && channel.Id != targetChannelId)
                throw new ChannelNameAlreadyExistException(channelName);

            return true;
        });
    }

    public static IRuleBuilderOptions<T, Tuple<Guid, Guid>> MustBeTheCreator<T>(this IRuleBuilder<T, Tuple<Guid, Guid>> ruleBuilder, IRepositoryManager repositoryManager)
    {
        return ruleBuilder.MustAsync(async (props, cancellationToken) =>
        {
            (Guid targetServerId, Guid deletedById) = props;

            var result = await repositoryManager.ServerRepository.GetByIdAsync(targetServerId);
            if (result is null)
                throw new ServerNotFoundException(targetServerId.ToString());

            if (result.CreatedById != deletedById)
                throw new UnauthorizedUserException(deletedById.ToString());

            return true;
        });
    }

    public static IRuleBuilderOptions<T, Tuple<Server, Guid>> MustBeTheCreatorOrAModerator<T>(this IRuleBuilder<T, Tuple<Server, Guid>> ruleBuilder, IRepositoryManager repositoryManager)
    {
        return ruleBuilder.MustAsync(async (props, cancellationToken) =>
        {
            (Server targetServer, Guid createdById) = props;

            var server = await repositoryManager.ServerRepository.GetByIdAsync(targetServer.Id);
            if (server is null)
                throw new ServerNotFoundException(targetServer.Id.ToString());

            if (server.CreatedById != createdById && !server.Moderators.Any(x => x.UserId == createdById))
                throw new UnauthorizedUserException(createdById.ToString());

            return true;
        });
    }

    public static IRuleBuilderOptions<T, Guid> MustBeExistingUser<T>(this IRuleBuilder<T, Guid> ruleBuilder, IRepositoryManager repositoryManager)
    {
        return ruleBuilder.MustAsync(async (userId, cancellationToken) =>
        {
            var result = await repositoryManager.UserRepository.GetByIdAsync(userId);
            if (result is null)
                throw new UserNotFoundException(userId.ToString());

            return true;
        });
    }

    public static IRuleBuilderOptions<T, Tuple<Server, Guid>> MustBeTheCreator<T>(this IRuleBuilder<T, Tuple<Server, Guid>> ruleBuilder, IRepositoryManager repositoryManager)
    {
        return ruleBuilder.MustAsync(async (props, cancellationToken) =>
        {
            (Server targetServer, Guid addedById) = props;

            var server = await repositoryManager.ServerRepository.GetByIdAsync(targetServer.Id);
            if (server is null)
                throw new ServerNotFoundException(targetServer.Id.ToString());

            if (server.CreatedById != addedById)
                throw new UnauthorizedUserException(addedById.ToString());

            return true;
        });
    }

    public static IRuleBuilderOptions<T, Tuple<Server, Guid>> MustBeExistingMember<T>(this IRuleBuilder<T, Tuple<Server, Guid>> ruleBuilder, IRepositoryManager repositoryManager)
    {
        return ruleBuilder.MustAsync(async (props, cancellationToken) =>
        {
            (Server targetServer, Guid userId) = props;

            var server = await repositoryManager.ServerRepository.GetByIdAsync(targetServer.Id);
            if (server is null)
                throw new ServerNotFoundException(targetServer.Id.ToString());

            if (!server.Members.Any(x => x.UserId == userId))
                throw new UserIsNotAMemberException(userId.ToString());

            return true;
        });
    }

    public static IRuleBuilderOptions<T, Tuple<Server, Guid>> MustBeExistingModerator<T>(this IRuleBuilder<T, Tuple<Server, Guid>> ruleBuilder, IRepositoryManager repositoryManager)
    {
        return ruleBuilder.MustAsync(async (props, cancellationToken) =>
        {
            (Server targetServer, Guid userId) = props;

            var server = await repositoryManager.ServerRepository.GetByIdAsync(targetServer.Id);
            if (server is null)
                throw new ServerNotFoundException(targetServer.Id.ToString());

            if (!server.Moderators.Any(x => x.UserId == userId))
                throw new UserIsNotAModeratorException(userId.ToString());

            return true;
        });
    }

    public static IRuleBuilderOptions<T, Tuple<Server, Guid>> MustNotBeExistingMember<T>(this IRuleBuilder<T, Tuple<Server, Guid>> ruleBuilder, IRepositoryManager repositoryManager)
    {
        return ruleBuilder.MustAsync(async (props, cancellationToken) =>
        {
            (Server targetServer, Guid userId) = props;

            var server = await repositoryManager.ServerRepository.GetByIdAsync(targetServer.Id);
            if (server is null)
                throw new ServerNotFoundException(targetServer.Id.ToString());

            if (server.Members.Any(x => x.UserId == userId) || server.CreatedById == userId)
                throw new UserIsAlreadyAMemberException(userId.ToString());

            return true;
        });
    }

    public static IRuleBuilderOptions<T, Tuple<Server, Guid>> MustNotBeExistingModerator<T>(this IRuleBuilder<T, Tuple<Server, Guid>> ruleBuilder, IRepositoryManager repositoryManager)
    {
        return ruleBuilder.MustAsync(async (props, cancellationToken) =>
        {
            (Server targetServer, Guid userId) = props;

            var server = await repositoryManager.ServerRepository.GetByIdAsync(targetServer.Id);
            if (server is null)
                throw new ServerNotFoundException(targetServer.Id.ToString());

            if (server.Moderators.Any(x => x.UserId == userId) || server.CreatedById == userId)
                throw new UserIsAlreadyAModeratorException(userId.ToString());

            return true;
        });
    }

    public static IRuleBuilderOptions<T, Tuple<Server, Guid>> MustBeExistingChannel<T>(this IRuleBuilder<T, Tuple<Server, Guid>> ruleBuilder, IRepositoryManager repositoryManager)
    {
        return ruleBuilder.MustAsync(async (props, cancellationToken) =>
        {
            (Server targetServer, Guid targetChannelId) = props;

            var server = await repositoryManager.ServerRepository.GetByIdAsync(targetServer.Id);
            if (server is null)
                throw new ServerNotFoundException(targetServer.Id.ToString());

            if (!server.Channels.Any(x => x.Id == targetChannelId))
                throw new ChannelNotFoundException(targetChannelId.ToString());

            return true;
        });
    }

    public static IRuleBuilderOptions<T, Tuple<Guid, Guid, Guid>> MustBeTheCreatorOrMemberOfTheChannel<T>(this IRuleBuilder<T, Tuple<Guid, Guid, Guid>> ruleBuilder, IRepositoryManager repositoryManager)
    {
        return ruleBuilder.MustAsync(async (props, cancellationToken) =>
        {
            (Guid serverId, Guid channelId, Guid senderId) = props;

            var server = await repositoryManager.ServerRepository.GetByIdAsync(serverId);
            if (server is null)
                throw new ServerNotFoundException(serverId.ToString());

            Channel? channel = server.Channels.FirstOrDefault(x => x.Id == channelId);
            if (channel is null)
                throw new ChannelNotFoundException(channelId.ToString());

            if (server.CreatedById != senderId && !channel.Members.Any(x => x == senderId))
                throw new UnauthorizedUserException(senderId.ToString());

            return true;
        });
    }

    public static IRuleBuilderOptions<T, Tuple<Server, Guid, Guid>> MustNotBeExistingMemberOfTheChannel<T>(this IRuleBuilder<T, Tuple<Server, Guid, Guid>> ruleBuilder, IRepositoryManager repositoryManager)
    {
        return ruleBuilder.MustAsync(async (props, cancellationToken) =>
        {
            (Server targetServer, Guid targetChannelId, Guid userId) = props;

            var server = await repositoryManager.ServerRepository.GetByIdAsync(targetServer.Id);
            if (server is null)
                throw new ServerNotFoundException(targetServer.Id.ToString());

            Channel? channel = server.Channels.FirstOrDefault(x => x.Id == targetChannelId);
            if (channel is null)
                throw new ChannelNotFoundException(targetChannelId.ToString());

            if (channel.Members.Any(x => x == userId) || server.CreatedById == userId)
                throw new UserIsAlreadyAMemberException(userId.ToString());

            return true;
        });
    }
}
