using Chat.Application.Commands;
using Chat.Application.Validators;
using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.Exceptions;
using Chat.Domain.SeedWork;
using Moq;

namespace Chat.UnitTests.Validators;

public class DeleteServerCommandValidatorTests
{
    private readonly Mock<IRepositoryManager> _mockRepositoryManager;
    private readonly DeleteServerCommandValidator _validator;

    public DeleteServerCommandValidatorTests()
    {
        Mock<IServerRepository> mockServerRepository = new();
        _mockRepositoryManager = new Mock<IRepositoryManager>();
        _mockRepositoryManager.Setup(x => x.ServerRepository).Returns(mockServerRepository.Object);
        _validator = new DeleteServerCommandValidator(_mockRepositoryManager.Object);
    }

    [Fact]
    public async Task TargetServerId_IsEmpty_ReturnsError()
    {
        // Arrange
        DeleteServerCommand command = new(Guid.Empty, "deleter@example.co");
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new Server("Server Name", "Short Description", "Long Description", command.DeleterEmail, ""));

        // Act
        var result = await _validator.ValidateAsync(command, It.IsAny<CancellationToken>());

        // Assert
        Assert.NotEmpty(result.Errors);
        Assert.True(result.Errors?.Any(x => x.PropertyName == "TargetServerId"));
    }

    [Fact]
    public async Task TargetServerId_IsInvalid_ThrowsServerNotFoundException()
    {
        // Arrange
        DeleteServerCommand command = new(Guid.NewGuid(), "deleter@example.co");
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Server)null!);

        // Act & Assert
        await Assert.ThrowsAsync<ServerNotFoundException>(() => _validator.ValidateAsync(command, It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task DeleterEmail_IsEmpty_ReturnsAnError()
    {
        // Arrange
        DeleteServerCommand command = new(Guid.NewGuid(), "");
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new Server("Server Name", "Short Description", "Long Description", command.DeleterEmail, ""));

        // Act
        var result = await _validator.ValidateAsync(command, It.IsAny<CancellationToken>());

        // Assert
        Assert.NotEmpty(result.Errors);
        Assert.True(result.Errors?.Any(x => x.PropertyName == "DeleterEmail"));
    }

    [Fact]
    public async Task DeleterEmail_IsNotValidEmailAddress_ReturnsAnError()
    {
        // Arrange
        DeleteServerCommand command = new(Guid.NewGuid(), "notvalidemail");
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new Server("Server Name", "Short Description", "Long Description", command.DeleterEmail, ""));

        // Act
        var result = await _validator.ValidateAsync(command, It.IsAny<CancellationToken>());

        // Assert
        Assert.NotEmpty(result.Errors);
        Assert.True(result.Errors?.Any(x => x.PropertyName == "DeleterEmail"));
    }

    [Fact]
    public async Task DeleterEmail_NotTheSameWithCreatorEmail_ReturnsAnError()
    {
        // Arrange
        DeleteServerCommand command = new(Guid.NewGuid(), "different_email@example.com");
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new Server("Server Name", "Short Description", "Long Description", "creator@example.com", ""));

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedServerDeleterException>(() => _validator.ValidateAsync(command, It.IsAny<CancellationToken>()));
    }
}
