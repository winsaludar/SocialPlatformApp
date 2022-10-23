using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.Exceptions;
using Chat.Domain.SeedWork;
using FluentValidation;

namespace Chat.Application.Extensions;

public static class FluentValidationExtensions
{
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
}
