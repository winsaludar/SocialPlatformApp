using Chat.Application.Commands;
using Chat.Application.Validators;
using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.Exceptions;
using Chat.Domain.SeedWork;
using Moq;

namespace Chat.UnitTests.Validators;

public class UpdateChannelCommandValidatorTests
{
    private readonly Mock<IRepositoryManager> _mockRepositoryManager;
    private readonly UpdateChannelCommandValidator _validator;

    public UpdateChannelCommandValidatorTests()
    {
        Mock<IServerRepository> mockServerRepository = new();
        _mockRepositoryManager = new Mock<IRepositoryManager>();
        _mockRepositoryManager.Setup(x => x.ServerRepository).Returns(mockServerRepository.Object);
        _validator = new UpdateChannelCommandValidator(_mockRepositoryManager.Object);
    }

    [Fact]
    public async Task TargetServerId_IsEmpty_ReturnsError()
    {
        // Arrange
        Guid channelId = Guid.NewGuid();
        Server targetServer = new("Target Server", "Short Desc", "Long Desc", "creator@example.com", "");
        targetServer.AddChannel(channelId, "Target Channel", DateTime.UtcNow);
        UpdateChannelCommand command = new(Guid.Empty, channelId, "Test Channel", "creator@example.com");
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
        UpdateChannelCommand command = new(Guid.Empty, Guid.NewGuid(), "Test Channel", "creator@example.com");
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
        UpdateChannelCommand command = new(Guid.NewGuid(), Guid.Empty, "Test Channel", "creator@example.com");
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
        UpdateChannelCommand command = new(Guid.NewGuid(), Guid.NewGuid(), "Test Channel", "creator@example.com");
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(targetServer);

        // Act & Assert
        await Assert.ThrowsAsync<ChannelNotFoundException>(() => _validator.ValidateAsync(command, It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task Name_IsEmpty_ReturnsError()
    {
        // Arrange
        Guid channelId = Guid.NewGuid();
        Server targetServer = new("Target Server", "Short Desc", "Long Desc", "creator@example.com", "");
        targetServer.AddChannel(channelId, "Target Channel", DateTime.UtcNow);
        UpdateChannelCommand command = new(Guid.NewGuid(), channelId, "", "creator@example.com");
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(targetServer);

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
        Guid channelId = Guid.NewGuid();
        Server targetServer = new("Target Server", "Short Desc", "Long Desc", "creator@example.com", "");
        targetServer.AddChannel(channelId, "Target Channel", DateTime.UtcNow);
        UpdateChannelCommand command = new(Guid.NewGuid(), channelId, name, "creator@example.com");
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(targetServer);

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
        Guid targetChannelId = Guid.NewGuid();
        Guid existingNameChannelId = Guid.NewGuid();
        Server targetServer = new("Target Server", "Short Desc", "Long Desc", "creator@example.com", "");
        targetServer.AddChannel(targetChannelId, "Target Channel", DateTime.UtcNow);
        targetServer.AddChannel(existingNameChannelId, "Existing Channel", DateTime.UtcNow);
        UpdateChannelCommand command = new(Guid.NewGuid(), targetChannelId, "Existing Channel", "creator@example.com");
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(targetServer);

        // Act & Assert
        await Assert.ThrowsAsync<ChannelNameAlreadyExistException>(() => _validator.ValidateAsync(command, It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task UpdatedBy_IsEmpty_ReturnsError()
    {
        // Arrange
        Guid channelId = Guid.NewGuid();
        Server targetServer = new("Target Server", "Short Desc", "Long Desc", "creator@example.com", "");
        targetServer.AddChannel(channelId, "Target Channel", DateTime.UtcNow);
        UpdateChannelCommand command = new(Guid.NewGuid(), channelId, "Channel Name", "");
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(targetServer);

        // Act
        var result = await _validator.ValidateAsync(command, It.IsAny<CancellationToken>());

        // Assert
        Assert.NotEmpty(result.Errors);
        Assert.True(result.Errors?.Any(x => x.PropertyName == "UpdatedBy"));
    }

    [Fact]
    public async Task UpdatedBy_IsNotEmailAddress_ReturnsError()
    {
        // Arrange
        Guid channelId = Guid.NewGuid();
        Server targetServer = new("Target Server", "Short Desc", "Long Desc", "creator@example.com", "");
        targetServer.AddChannel(channelId, "Target Channel", DateTime.UtcNow);
        UpdateChannelCommand command = new(Guid.NewGuid(), channelId, "Channel Name", "notvalidemail.com");
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(targetServer);

        // Act
        var result = await _validator.ValidateAsync(command, It.IsAny<CancellationToken>());

        // Assert
        Assert.NotEmpty(result.Errors);
        Assert.True(result.Errors?.Any(x => x.PropertyName == "UpdatedBy"));
    }
}
