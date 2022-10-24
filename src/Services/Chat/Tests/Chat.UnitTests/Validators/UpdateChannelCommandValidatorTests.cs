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
    public async Task TargetServer_IsInvalid_ThrowsServerNotFoundException()
    {
        // Arrange
        Server targetServer = GetTargetServer();
        UpdateChannelCommand command = new(targetServer, Guid.NewGuid(), "Test Channel", true, targetServer.CreatedById);
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Server)null!);

        // Act & Assert
        await Assert.ThrowsAsync<ServerNotFoundException>(() => _validator.ValidateAsync(command, It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task TargetChannelId_IsEmpty_ReturnsError()
    {
        // Arrange
        Server targetServer = GetTargetServer();
        targetServer.AddChannel(Guid.Empty, "Fake Channel", true, targetServer.CreatedById, DateTime.UtcNow);
        UpdateChannelCommand command = new(targetServer, Guid.Empty, "Test Channel", true, targetServer.CreatedById);
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(targetServer);

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
        Server targetServer = GetTargetServer();
        targetServer.AddChannel(Guid.NewGuid(), "Different Channel", true, targetServer.CreatedById, DateTime.UtcNow);
        UpdateChannelCommand command = new(targetServer, Guid.NewGuid(), "Test Channel", true, targetServer.CreatedById);
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(targetServer);

        // Act & Assert
        await Assert.ThrowsAsync<ChannelNotFoundException>(() => _validator.ValidateAsync(command, It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task Name_IsEmpty_ReturnsError()
    {
        // Arrange
        Guid channelId = Guid.NewGuid();
        Server targetServer = GetTargetServer();
        targetServer.AddChannel(channelId, "Target Channel", true, targetServer.CreatedById, DateTime.UtcNow);
        UpdateChannelCommand command = new(targetServer, channelId, "", true, targetServer.CreatedById);
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(targetServer);

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
        Server targetServer = GetTargetServer();
        targetServer.AddChannel(channelId, "Target Channel", true, targetServer.CreatedById, DateTime.UtcNow);
        UpdateChannelCommand command = new(targetServer, channelId, name, true, targetServer.CreatedById);
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(targetServer);

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
        Server targetServer = GetTargetServer();
        targetServer.AddChannel(targetChannelId, "Target Channel", true, targetServer.CreatedById, DateTime.UtcNow);
        targetServer.AddChannel(existingNameChannelId, "Existing Channel", true, targetServer.CreatedById, DateTime.UtcNow);
        UpdateChannelCommand command = new(targetServer, targetChannelId, "Existing Channel", true, Guid.NewGuid());
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(targetServer);

        // Act & Assert
        await Assert.ThrowsAsync<ChannelNameAlreadyExistException>(() => _validator.ValidateAsync(command, It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task UpdatedById_IsEmpty_ReturnsError()
    {
        // Arrange
        Guid channelId = Guid.NewGuid();
        Server targetServer = GetTargetServer();
        targetServer.AddChannel(channelId, "Target Channel", true, targetServer.CreatedById, DateTime.UtcNow);
        UpdateChannelCommand command = new(targetServer, channelId, "Channel Name", true, Guid.Empty);
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(targetServer);

        // Act
        var result = await _validator.ValidateAsync(command, It.IsAny<CancellationToken>());

        // Assert
        Assert.NotEmpty(result.Errors);
        Assert.True(result.Errors?.Any(x => x.PropertyName == "UpdatedById"));
    }

    [Fact]
    public async Task UpdatedById_IsNotTheCreatorAndNotOneOfTheModerators_ThrowsUnauthorizedUserException()
    {
        // Arrange
        Guid channelId = Guid.NewGuid();
        Server targetServer = GetTargetServer();
        targetServer.AddChannel(channelId, "Target Channel", true, targetServer.CreatedById, DateTime.UtcNow);
        UpdateChannelCommand command = new(targetServer, channelId, "Channel Name", true, Guid.NewGuid());
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(targetServer);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedUserException>(() => _validator.ValidateAsync(command, It.IsAny<CancellationToken>()));
    }

    private static Server GetTargetServer()
    {
        Server targetServer = new("Target Server", "Short Desc", "Long Desc", "creator@example.com", "");
        targetServer.SetId(Guid.NewGuid());

        return targetServer;
    }
}
