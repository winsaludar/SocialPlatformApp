using Chat.Application.Commands;
using Chat.Application.IntegrationEvents;
using Chat.Application.Validators;
using Chat.Domain.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace Chat.UnitTests.IntegrationEvents;

public class UserRegisteredSuccessfulIntegrationEventHandlerTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly Mock<ILogger<UserRegisteredSuccessfulIntegrationEventHandler>> _mockLogger;
    private readonly InlineValidator<CreateUserCommand> _createUserValidator;
    private readonly UserRegisteredSuccessfulIntegrationEventHandler _handler;

    public UserRegisteredSuccessfulIntegrationEventHandlerTests()
    {
        _mockMediator = new Mock<IMediator>();
        _createUserValidator = new InlineValidator<CreateUserCommand>();

        Mock<IValidatorManager> mockValidatorManager = new();
        mockValidatorManager.Setup(x => x.CreateUserCommandValidator).Returns(_createUserValidator);

        _mockLogger = new Mock<ILogger<UserRegisteredSuccessfulIntegrationEventHandler>>();

        _handler = new UserRegisteredSuccessfulIntegrationEventHandler(_mockMediator.Object, mockValidatorManager.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_ValidationResultIsInvalid_DoNothing()
    {
        // Assert
        UserRegisteredSuccessfulIntegrationEvent @event = new(Guid.NewGuid(), "", "email@example.com");
        _createUserValidator.RuleFor(x => x.Username).Must(name => false);

        // Act
        await _handler.Handle(@event);

        // Assert
        _mockMediator.Verify(x => x.Send(It.IsAny<CreateUserCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ValidatorThrowsException_DoNothing()
    {
        // Assert
        UserRegisteredSuccessfulIntegrationEvent @event = new(Guid.NewGuid(), "", "email@example.com");
        _createUserValidator.RuleFor(x => x.Username).Must(name => throw new UsernameAlreadyExistException(name));

        // Act
        await _handler.Handle(@event);

        // Assert
        _mockMediator.Verify(x => x.Send(It.IsAny<CreateUserCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ValidationResultIsValid_CallsMediator()
    {
        // Assert
        UserRegisteredSuccessfulIntegrationEvent @event = new(Guid.NewGuid(), "Username", "email@example.com");
        _createUserValidator.RuleFor(x => x.Username).Must(name => true);
        _mockMediator.Setup(x => x.Send(It.IsAny<CreateUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Guid.NewGuid());

        // Act
        await _handler.Handle(@event);

        // Assert
        _mockMediator.Verify(x => x.Send(It.IsAny<CreateUserCommand>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
