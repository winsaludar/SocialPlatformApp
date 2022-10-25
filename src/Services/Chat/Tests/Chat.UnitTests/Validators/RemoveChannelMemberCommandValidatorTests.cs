using Chat.Application.Commands;
using Chat.Application.Validators;
using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.Aggregates.UserAggregate;
using Chat.Domain.Exceptions;
using Chat.Domain.SeedWork;
using Moq;

namespace Chat.UnitTests.Validators;

public class RemoveChannelMemberCommandValidatorTests
{
    private readonly Mock<IRepositoryManager> _mockRepositoryManager;
    private readonly RemoveChannelMemberCommandValidator _validator;

    public RemoveChannelMemberCommandValidatorTests()
    {
        Mock<IServerRepository> mockServerRepository = new();
        Mock<IUserRepository> mockUserRepository = new();
        _mockRepositoryManager = new Mock<IRepositoryManager>();
        _mockRepositoryManager.Setup(x => x.ServerRepository).Returns(mockServerRepository.Object);
        _mockRepositoryManager.Setup(x => x.UserRepository).Returns(mockUserRepository.Object);
        _validator = new RemoveChannelMemberCommandValidator(_mockRepositoryManager.Object);
    }

    [Fact]
    public async Task TargetServer_IsInvalid_ThrowsServerNotFoundException()
    {
        // Arrange
        Server targetServer = GetTargetServer();
        RemoveChannelMemberCommand command = new(targetServer, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Server)null!);

        // Act & Assert
        await Assert.ThrowsAsync<ServerNotFoundException>(() => _validator.ValidateAsync(command, It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task ChannelId_IsEmpty_ReturnsAnError()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        Guid channelId = Guid.Empty;
        Server targetServer = GetTargetServer();
        targetServer.AddChannel(channelId, "notexisting", true, targetServer.CreatedById, DateTime.UtcNow);
        targetServer.AddMember(userId, "user", DateTime.UtcNow);
        targetServer.AddChannelMember(channelId, userId);
        RemoveChannelMemberCommand command = new(targetServer, channelId, userId, targetServer.CreatedById);
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(targetServer);
        _mockRepositoryManager.Setup(x => x.UserRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(GetUser(targetServer.CreatedById));

        // Act
        var result = await _validator.ValidateAsync(command, It.IsAny<CancellationToken>());

        // Assert
        Assert.NotEmpty(result.Errors);
        Assert.True(result.Errors?.Any(x => x.PropertyName == "ChannelId"));
    }

    [Fact]
    public async Task UserId_IsEmpty_ReturnsAnError()
    {
        // Arrange
        Guid userId = Guid.Empty;
        Guid channelId = Guid.NewGuid();
        Server targetServer = GetTargetServer();
        targetServer.AddChannel(channelId, "channel", true, targetServer.CreatedById, DateTime.UtcNow);
        targetServer.AddMember(userId, "notexisting", DateTime.UtcNow);
        targetServer.AddChannelMember(channelId, userId);
        RemoveChannelMemberCommand command = new(targetServer, channelId, userId, targetServer.CreatedById);
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(targetServer);
        _mockRepositoryManager.Setup(x => x.UserRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(GetUser(targetServer.CreatedById));

        // Act
        var result = await _validator.ValidateAsync(command, It.IsAny<CancellationToken>());

        // Assert
        Assert.NotEmpty(result.Errors);
        Assert.True(result.Errors?.Any(x => x.PropertyName == "UserId"));
    }

    [Fact]
    public async Task RemovedById_IsNotTheCreatorOrAModerator_ThrowsUnauthorizedUserException()
    {
        // Arrange
        Guid channelId = Guid.NewGuid();
        Guid userId = Guid.NewGuid();
        Server targetServer = GetTargetServer();
        RemoveChannelMemberCommand command = new(targetServer, channelId, userId, Guid.NewGuid());
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(targetServer);
        _mockRepositoryManager.Setup(x => x.UserRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(GetUser(Guid.NewGuid()));

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedUserException>(() => _validator.ValidateAsync(command, It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task ChannelId_IsInvalid_ThrowsChannelNotFoundException()
    {
        // Arrange
        Guid channelId = Guid.NewGuid();
        Guid userId = Guid.NewGuid();
        Server targetServer = GetTargetServer();
        RemoveChannelMemberCommand command = new(targetServer, channelId, userId, targetServer.CreatedById);
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(targetServer);
        _mockRepositoryManager.Setup(x => x.UserRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(GetUser(targetServer.CreatedById));

        // Act & Assert
        await Assert.ThrowsAsync<ChannelNotFoundException>(() => _validator.ValidateAsync(command, It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task User_IsNotAMemberOfTheChannel_ThrowsUserIsNotAMemberException()
    {
        // Arrange
        Guid channelId = Guid.NewGuid();
        Guid userId = Guid.NewGuid();
        Server targetServer = GetTargetServer();
        targetServer.AddMember(userId, "user", DateTime.UtcNow);
        targetServer.AddChannel(channelId, "channel", true, targetServer.CreatedById, DateTime.UtcNow);
        RemoveChannelMemberCommand command = new(targetServer, channelId, userId, targetServer.CreatedById);
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(targetServer);
        _mockRepositoryManager.Setup(x => x.UserRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(GetUser(targetServer.CreatedById));

        // Act & Assert
        await Assert.ThrowsAsync<UserIsNotAMemberException>(() => _validator.ValidateAsync(command, It.IsAny<CancellationToken>()));
    }

    private static Server GetTargetServer()
    {
        Server targetServer = new("Target Server", "Short Desc", "Long Desc", "creator@example.com", "");
        targetServer.SetCreatedById(Guid.NewGuid());
        targetServer.SetId(Guid.NewGuid());

        return targetServer;
    }

    private static User GetUser(Guid userId)
    {
        User user = new(Guid.NewGuid(), "user", "user@example.com");
        user.SetId(userId);

        return user;
    }
}
