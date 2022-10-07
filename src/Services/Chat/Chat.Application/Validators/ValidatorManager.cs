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
    }

    public AbstractValidator<CreateServerCommand> CreateServerCommandValidator => _lazyCreateServerCommandValidator.Value;
    public AbstractValidator<CreateUserCommand> CreateUserCommandValidator => _lazyCreateUserCommandValidator.Value;
    public AbstractValidator<GetServersQuery> GetServersQueryValidator => _lazyGetServersQueryValidator.Value;
    public AbstractValidator<UpdateServerCommand> UpdateServerCommandValidator => _lazyUpdateServerCommandValidator.Value;
    public AbstractValidator<DeleteServerCommand> DeleteServerCommandValidator => _lazyDeleteServerCommandValidator.Value;
    public AbstractValidator<CreateChannelCommand> CreateChannelCommandValidator => _lazyCreateChannelCommandValidator.Value;
    public AbstractValidator<GetChannelsQuery> GetChannelsQueryValidator => _lazyGetChannelsQueryValidator.Value;
    public AbstractValidator<UpdateChannelCommand> UpdateChannelCommandValidator => _lazyUpdateChannelCommandValidator.Value;
}
