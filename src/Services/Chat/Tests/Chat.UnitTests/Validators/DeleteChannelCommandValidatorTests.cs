using Chat.Application.Commands;
using Chat.Application.Validators;
using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.Exceptions;
using Chat.Domain.SeedWork;
using Moq;

namespace Chat.UnitTests.Validators;

public class DeleteChannelCommandValidatorTests
{
    private readonly Mock<IRepositoryManager> _mockRepositoryManager;
    private readonly DeleteChannelCommandValidator _validator;

    public DeleteChannelCommandValidatorTests()
    {
        Mock<IServerRepository> mockServerRepository = new();
        _mockRepositoryManager = new Mock<IRepositoryManager>();
        _mockRepositoryManager.Setup(x => x.ServerRepository).Returns(mockServerRepository.Object);
        _validator = new DeleteChannelCommandValidator(_mockRepositoryManager.Object);
    }

    [Fact]
    public async Task TargetServerId_IsEmpty_ReturnsError()
    {
        // Arrange
        Guid channelId = Guid.NewGuid();
        Server targetServer = new("Target Server", "Short Desc", "Long Desc", "creator@example.com", "");
        targetServer.AddChannel(channelId, "Target Channel", DateTime.UtcNow);
        DeleteChannelCommand command = new(Guid.Empty, channelId);
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(targetServer);

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
        DeleteChannelCommand command = new(Guid.Empty, Guid.NewGuid());
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Server)null!);

        // Act & Assert
        await Assert.ThrowsAsync<ServerNotFoundException>(() => _validator.ValidateAsync(command, It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task TargetChannelId_IsEmpty_ReturnsError()
    {
        // Arrange
        Server targetServer = new("Target Server", "Short Desc", "Long Desc", "creator@example.com", "");
        targetServer.AddChannel(Guid.Empty, "Fake Channel", DateTime.UtcNow);
        DeleteChannelCommand command = new(Guid.NewGuid(), Guid.Empty);
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(targetServer);

        // Act
        var result = await _validator.ValidateAsync(command, It.IsAny<CancellationToken>());

        // Assert
        Assert.NotEmpty(result.Errors);
        Assert.True(result.Errors?.Any(x => x.PropertyName == "TargetChannelId"));
    }

    [Fact]
    public async Task TargetChannelId_IsInvalid_ThrowsChannelNotFoundException()
    {
        // Arrange
        Server targetServer = new("Target Server", "Short Desc", "Long Desc", "creator@example.com", "");
        targetServer.AddChannel(Guid.NewGuid(), "Different Channel", DateTime.UtcNow);
        DeleteChannelCommand command = new(Guid.NewGuid(), Guid.NewGuid());
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(targetServer);

        // Act & Assert
        await Assert.ThrowsAsync<ChannelNotFoundException>(() => _validator.ValidateAsync(command, It.IsAny<CancellationToken>()));
    }
}
