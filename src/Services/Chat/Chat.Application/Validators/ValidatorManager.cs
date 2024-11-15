﻿using Chat.Application.Commands;
using Chat.Application.Queries;
using Chat.Domain.SeedWork;
using FluentValidation;

namespace Chat.Application.Validators;

public class ValidatorManager : IValidatorManager
{
    private readonly Lazy<CreateServerCommandValidator> _lazyCreateServerCommandValidator;
    private readonly Lazy<CreateUserCommandValidator> _lazyCreateUserCommandValidator;
    private readonly Lazy<GetServersQueryValidator> _lazyGetServersQueryValidator;
    private readonly Lazy<UpdateServerCommandValidator> _lazyUpdateServerCommandValidator;
    private readonly Lazy<DeleteServerCommandValidator> _lazyDeleteServerCommandValidator;
    private readonly Lazy<CreateChannelCommandValidator> _lazyCreateChannelCommandValidator;
    private readonly Lazy<GetChannelsQueryValidator> _lazyGetChannelsQueryValidator;
    private readonly Lazy<UpdateChannelCommandValidator> _lazyUpdateChannelCommandValidator;
    private readonly Lazy<DeleteChannelCommandValidator> _lazyDeleteChannelCommandValidator;
    private readonly Lazy<AddMessageCommandValidator> _lazyAddMessageCommandValidator;
    private readonly Lazy<JoinServerCommandValidator> _lazyJoinServerCommandValidator;
    private readonly Lazy<LeaveServerCommandValidator> _lazyLeaveServerCommandValidator;
    private readonly Lazy<ChangeUsernameCommandValidator> _lazyChangeUsernameCommandValidator;
    private readonly Lazy<AddModeratorCommandValidator> _lazyAddModeratorCommandValidator;
    private readonly Lazy<RemoveModeratorCommandValidator> _lazyRemoveModeratorCommandValidator;
    private readonly Lazy<AddChannelMemberCommandValidator> _lazyAddChannelMemberCommandValidator;
    private readonly Lazy<RemoveChannelMemberCommandValidator> _lazyRemoveChannelMemberCommandValidator;
    private readonly Lazy<GetUserServersQueryValidator> _lazyGetUserServersQueryValidator;

    public ValidatorManager(IRepositoryManager repositoryManager)
    {
        _lazyCreateServerCommandValidator = new Lazy<CreateServerCommandValidator>(() => new CreateServerCommandValidator(repositoryManager));
        _lazyCreateUserCommandValidator = new Lazy<CreateUserCommandValidator>(() => new CreateUserCommandValidator(repositoryManager));
        _lazyGetServersQueryValidator = new Lazy<GetServersQueryValidator>(() => new GetServersQueryValidator());
        _lazyUpdateServerCommandValidator = new Lazy<UpdateServerCommandValidator>(() => new UpdateServerCommandValidator(repositoryManager));
        _lazyDeleteServerCommandValidator = new Lazy<DeleteServerCommandValidator>(() => new DeleteServerCommandValidator(repositoryManager));
        _lazyCreateChannelCommandValidator = new Lazy<CreateChannelCommandValidator>(() => new CreateChannelCommandValidator(repositoryManager));
        _lazyGetChannelsQueryValidator = new Lazy<GetChannelsQueryValidator>(() => new GetChannelsQueryValidator(repositoryManager));
        _lazyUpdateChannelCommandValidator = new Lazy<UpdateChannelCommandValidator>(() => new UpdateChannelCommandValidator(repositoryManager));
        _lazyDeleteChannelCommandValidator = new Lazy<DeleteChannelCommandValidator>(() => new DeleteChannelCommandValidator(repositoryManager));
        _lazyAddMessageCommandValidator = new Lazy<AddMessageCommandValidator>(() => new AddMessageCommandValidator(repositoryManager));
        _lazyJoinServerCommandValidator = new Lazy<JoinServerCommandValidator>(() => new JoinServerCommandValidator(repositoryManager));
        _lazyLeaveServerCommandValidator = new Lazy<LeaveServerCommandValidator>(() => new LeaveServerCommandValidator(repositoryManager));
        _lazyChangeUsernameCommandValidator = new Lazy<ChangeUsernameCommandValidator>(() => new ChangeUsernameCommandValidator(repositoryManager));
        _lazyAddModeratorCommandValidator = new Lazy<AddModeratorCommandValidator>(() => new AddModeratorCommandValidator(repositoryManager));
        _lazyRemoveModeratorCommandValidator = new Lazy<RemoveModeratorCommandValidator>(() => new RemoveModeratorCommandValidator(repositoryManager));
        _lazyAddChannelMemberCommandValidator = new Lazy<AddChannelMemberCommandValidator>(() => new AddChannelMemberCommandValidator(repositoryManager));
        _lazyRemoveChannelMemberCommandValidator = new Lazy<RemoveChannelMemberCommandValidator>(() => new RemoveChannelMemberCommandValidator(repositoryManager));
        _lazyGetUserServersQueryValidator = new Lazy<GetUserServersQueryValidator>(() => new GetUserServersQueryValidator(repositoryManager));
    }

    public AbstractValidator<CreateServerCommand> CreateServerCommandValidator => _lazyCreateServerCommandValidator.Value;
    public AbstractValidator<CreateUserCommand> CreateUserCommandValidator => _lazyCreateUserCommandValidator.Value;
    public AbstractValidator<GetServersQuery> GetServersQueryValidator => _lazyGetServersQueryValidator.Value;
    public AbstractValidator<UpdateServerCommand> UpdateServerCommandValidator => _lazyUpdateServerCommandValidator.Value;
    public AbstractValidator<DeleteServerCommand> DeleteServerCommandValidator => _lazyDeleteServerCommandValidator.Value;
    public AbstractValidator<CreateChannelCommand> CreateChannelCommandValidator => _lazyCreateChannelCommandValidator.Value;
    public AbstractValidator<GetChannelsQuery> GetChannelsQueryValidator => _lazyGetChannelsQueryValidator.Value;
    public AbstractValidator<UpdateChannelCommand> UpdateChannelCommandValidator => _lazyUpdateChannelCommandValidator.Value;
    public AbstractValidator<DeleteChannelCommand> DeleteChannelCommandValidator => _lazyDeleteChannelCommandValidator.Value;
    public AbstractValidator<AddMessageCommand> AddMessageCommandValidator => _lazyAddMessageCommandValidator.Value;
    public AbstractValidator<JoinServerCommand> JoinServerCommandValidator => _lazyJoinServerCommandValidator.Value;
    public AbstractValidator<LeaveServerCommand> LeaveServerCommandValidator => _lazyLeaveServerCommandValidator.Value;
    public AbstractValidator<ChangeUsernameCommand> ChangeUsernameCommandValidator => _lazyChangeUsernameCommandValidator.Value;
    public AbstractValidator<AddModeratorCommand> AddModeratorCommandValidator => _lazyAddModeratorCommandValidator.Value;
    public AbstractValidator<RemoveModeratorCommand> RemoveModeratorCommandValidator => _lazyRemoveModeratorCommandValidator.Value;
    public AbstractValidator<AddChannelMemberCommand> AddChannelMemberCommandValidator => _lazyAddChannelMemberCommandValidator.Value;
    public AbstractValidator<RemoveChannelMemberCommand> RemoveChannelMemberCommandValidator => _lazyRemoveChannelMemberCommandValidator.Value;
    public AbstractValidator<GetUserServersQuery> GetUserServersQueryValidator => _lazyGetUserServersQueryValidator.Value;
}
