﻿using Chat.Application.Commands;
using Chat.Application.Queries;
using FluentValidation;

namespace Chat.Application.Validators;

public interface IValidatorManager
{
    public AbstractValidator<CreateServerCommand> CreateServerCommandValidator { get; }
    public AbstractValidator<GetServersQuery> GetServersQueryValidator { get; }
}
