using Chat.Application.Commands;
using Chat.Application.Validators;
using EventBus.Core.Abstractions;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Chat.Application.IntegrationEvents;

public class UserRegisteredSuccessfulIntegrationEventHandler : IIntegrationEventHandler<UserRegisteredSuccessfulIntegrationEvent>
{
    private readonly IMediator _mediator;
    private readonly IValidatorManager _validationManager;
    private readonly ILogger<UserRegisteredSuccessfulIntegrationEventHandler> _logger;

    public UserRegisteredSuccessfulIntegrationEventHandler(
        IMediator mediator,
        IValidatorManager validationManager,
        ILogger<UserRegisteredSuccessfulIntegrationEventHandler> logger)
    {
        _mediator = mediator;
        _validationManager = validationManager;
        _logger = logger;
    }

    public async Task Handle(UserRegisteredSuccessfulIntegrationEvent @event)
    {
        try
        {
            CreateUserCommand command = new(@event.UserId, @event.Username, @event.Email, @event.Id);
            ValidationResult validationResult = await _validationManager.CreateUserCommandValidator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                validationResult.Errors.ForEach(x => _logger.LogError("Unable to create user! {PropertyName}: {ErrorMessage}", x.PropertyName, x.ErrorMessage));
                return;
            }

            var newId = await _mediator.Send(command);
            _logger.LogInformation("User successfully created with Guid: {Guid}", newId);
        }
        catch (Exception ex)
        {
            _logger.LogError("Unable to create user! {ErrorMessage}", ex.Message);
        }
    }
}
