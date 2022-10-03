using Chat.Application.Commands;
using Chat.Application.Validators;
using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.Exceptions;
using Chat.Domain.SeedWork;
using Moq;

namespace Chat.UnitTests.Validators;

public class CreateChannelCommandValidatorTests
{
    private readonly Mock<IRepositoryManager> _mockRepositoryManager;
    private readonly CreateChannelCommandValidator _validator;

    public CreateChannelCommandValidatorTests()
    {
        Mock<IServerRepository> mockServerRepository = new();
        _mockRepositoryManager = new Mock<IRepositoryManager>();
        _mockRepositoryManager.Setup(x => x.ServerRepository).Returns(mockServerRepository.Object);
        _validator = new CreateChannelCommandValidator(_mockRepositoryManager.Object);
    }

    [Fact]
    public async Task TargetServerId_IsEmpty_ReturnsError()
    {
        // Arrange
        CreateChannelCommand command = new(Guid.Empty, "Test Channel");
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new Server("Target Server", "Short Desc", "Long Desc", "creator@example.com", ""));

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
        CreateChannelCommand command = new(Guid.NewGuid(), "Test Channel");
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Server)null!);

        // Act & Assert
        await Assert.ThrowsAsync<ServerNotFoundException>(() => _validator.ValidateAsync(command, It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task Name_IsEmpty_ReturnsError()
    {
        // Arrange
        CreateChannelCommand command = new(Guid.NewGuid(), "");
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new Server("Target Server", "Short Desc", "Long Desc", "creator@example.com", ""));

        // Act
        var result = await _validator.ValidateAsync(command, It.IsAny<CancellationToken>());

        // Assert
        Assert.NotEmpty(result.Errors);
        Assert.True(result.Errors?.Any(x => x.PropertyName == "Name"));
    }

    [Fact]
    public async Task Name_ExceedsMaximumCharacterLength_ReturnsError()
    {
        // Arrange
        string name = "This is a long channel name that exceeds 50 characters in length 1234567890.";
        CreateChannelCommand command = new(Guid.NewGuid(), name);
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new Server("Target Server", "Short Desc", "Long Desc", "creator@example.com", ""));

        // Act
        var result = await _validator.ValidateAsync(command, It.IsAny<CancellationToken>());

        // Assert
        Assert.NotEmpty(result.Errors);
        Assert.True(result.Errors?.Any(x => x.PropertyName == "Name"));
    }

    [Fact]
    public async Task Name_AlreadyExist_ReturnsError()
    {
        // Arrange
        CreateChannelCommand command = new(Guid.NewGuid(), "Existing Channel");
        Server targetServer = new("Target Server", "Short Desc", "Long Desc", "creator@example.com", "");
        targetServer.AddChannel(Guid.NewGuid(), "Existing Channel", DateTime.UtcNow);
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(targetServer);

        // Act & Assert
        await Assert.ThrowsAsync<ChannelNameAlreadyExistException>(() => _validator.ValidateAsync(command, It.IsAny<CancellationToken>()));
    }
}
