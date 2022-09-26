using Chat.Application.Commands;
using Chat.Application.Queries;
using FluentValidation;

namespace Chat.Application.Validators;

public interface IValidatorManager
{
    public AbstractValidator<CreateServerCommand> CreateServerCommandValidator { get; }
    public AbstractValidator<CreateUserCommand> CreateUserCommandValidator { get; }
    public AbstractValidator<GetServersQuery> GetServersQueryValidator { get; }
    public AbstractValidator<UpdateServerCommand> UpdateServerCommandValidator { get; }
    public AbstractValidator<DeleteServerCommand> DeleteServerCommandValidator { get; }
}
