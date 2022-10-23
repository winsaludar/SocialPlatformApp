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
    public async Task TargetServer_IsInvalid_ThrowsServerNotFoundException()
    {
        // Arrange
        Server targetServer = GetTargetServer();
        DeleteChannelCommand command = new(targetServer, Guid.NewGuid(), targetServer.CreatedById);
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Server)null!);

        // Act & Assert
        await Assert.ThrowsAsync<ServerNotFoundException>(() => _validator.ValidateAsync(command, It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task TargetChannelId_IsEmpty_ReturnsError()
    {
        // Arrange
        Server targetServer = GetTargetServer();
        targetServer.AddChannel(Guid.Empty, "Fake Channel", Guid.NewGuid(), DateTime.UtcNow);
        DeleteChannelCommand command = new(targetServer, Guid.Empty, targetServer.CreatedById);
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
        targetServer.AddChannel(Guid.NewGuid(), "Different Channel", Guid.NewGuid(), DateTime.UtcNow);
        DeleteChannelCommand command = new(targetServer, Guid.NewGuid(), targetServer.CreatedById);
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(targetServer);

        // Act & Assert
        await Assert.ThrowsAsync<ChannelNotFoundException>(() => _validator.ValidateAsync(command, It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task DeletedById_IsEmpty_ReturnsError()
    {
        // Arrange
        Guid channelId = Guid.NewGuid();
        Server targetServer = GetTargetServer();
        targetServer.AddChannel(channelId, "Target Channel", targetServer.CreatedById, DateTime.UtcNow);
        DeleteChannelCommand command = new(targetServer, channelId, Guid.Empty);
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(targetServer);

        // Act
        var result = await _validator.ValidateAsync(command, It.IsAny<CancellationToken>());

        // Assert
        Assert.NotEmpty(result.Errors);
        Assert.True(result.Errors?.Any(x => x.PropertyName == "DeletedById"));
    }

    [Fact]
    public async Task DeletedById_IsNotTheCreatorAndNotOneOfTheModerators_ThrowsUnauthorizedUserException()
    {
        // Arrange
        Guid channelId = Guid.NewGuid();
        Server targetServer = GetTargetServer();
        targetServer.AddChannel(channelId, "Target Channel", targetServer.CreatedById, DateTime.UtcNow);
        DeleteChannelCommand command = new(targetServer, channelId, Guid.NewGuid());
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
