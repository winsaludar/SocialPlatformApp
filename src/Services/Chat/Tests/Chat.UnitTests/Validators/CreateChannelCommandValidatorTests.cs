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
    public async Task TargetServer_IsInvalid_ThrowsServerNotFoundException()
    {
        // Arrange
        Server targetServer = GetTargetServer();
        CreateChannelCommand command = new(targetServer, "Test Channel", true, Guid.NewGuid());
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Server)null!);

        // Act & Assert
        await Assert.ThrowsAsync<ServerNotFoundException>(() => _validator.ValidateAsync(command, It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task Name_IsEmpty_ReturnsError()
    {
        // Arrange
        Server targetServer = GetTargetServer();
        CreateChannelCommand command = new(targetServer, "", true, targetServer.CreatedById);
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(
            new Server("Target Server", "Short Desc", "Long Desc", "user@example.com", ""));

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
        Server targetServer = GetTargetServer();
        string name = "This is a long channel name that exceeds 50 characters in length 1234567890.";
        CreateChannelCommand command = new(targetServer, name, true, targetServer.CreatedById);
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(
            new Server("Target Server", "Short Desc", "Long Desc", "user@example.com", ""));

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
        Server targetServer = GetTargetServer();
        CreateChannelCommand command = new(targetServer, "Existing Channel", true, targetServer.CreatedById);
        targetServer.AddChannel(Guid.NewGuid(), "Existing Channel", true, Guid.NewGuid(), DateTime.UtcNow);
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(targetServer);

        // Act & Assert
        await Assert.ThrowsAsync<ChannelNameAlreadyExistException>(() => _validator.ValidateAsync(command, It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task CreatedById_IsEmpty_ReturnsError()
    {
        // Arrange
        Server targetServer = GetTargetServer();
        targetServer.AddModerator(Guid.Empty, DateTime.UtcNow);
        CreateChannelCommand command = new(targetServer, "Channel Name", true, Guid.Empty);
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(targetServer);

        // Act
        var result = await _validator.ValidateAsync(command, It.IsAny<CancellationToken>());

        // Assert
        Assert.NotEmpty(result.Errors);
        Assert.True(result.Errors?.Any(x => x.PropertyName == "CreatedById"));
    }

    [Fact]
    public async Task CreatedById_IsNotTheCreatorAndNotOneOfTheModerators_ThrowsUnauthorizedUserException()
    {
        // Arrange
        Server targetServer = GetTargetServer();
        CreateChannelCommand command = new(targetServer, "Channel Name", true, Guid.NewGuid());
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
