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

    public ValidatorManager(IRepositoryManager repositoryManager)
    {
        _lazyCreateServerCommandValidator = new Lazy<CreateServerCommandValidator>(() => new CreateServerCommandValidator(repositoryManager));
        _lazyCreateUserCommandValidator = new Lazy<CreateUserCommandValidator>(() => new CreateUserCommandValidator(repositoryManager));
        _lazyGetServersQueryValidator = new Lazy<GetServersQueryValidator>(() => new GetServersQueryValidator());
    }

    public AbstractValidator<CreateServerCommand> CreateServerCommandValidator => _lazyCreateServerCommandValidator.Value;
    public AbstractValidator<CreateUserCommand> CreateUserCommandValidator => _lazyCreateUserCommandValidator.Value;
    public AbstractValidator<GetServersQuery> GetServersQueryValidator => _lazyGetServersQueryValidator.Value;
}