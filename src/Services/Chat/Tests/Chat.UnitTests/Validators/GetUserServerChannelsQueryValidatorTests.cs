using Chat.Application.Queries;
using Chat.Application.Validators;
using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.Aggregates.UserAggregate;
using Chat.Domain.Exceptions;
using Chat.Domain.SeedWork;
using Moq;

namespace Chat.UnitTests.Validators;

public class GetUserServerChannelsQueryValidatorTests
{
    private readonly Mock<IRepositoryManager> _mockRepositoryManager;
    private readonly GetUserServerChannelsQueryValidator _validator;

    public GetUserServerChannelsQueryValidatorTests()
    {
        Mock<IServerRepository> mockServerRepository = new();
        Mock<IUserRepository> mockUserRepository = new();
        _mockRepositoryManager = new Mock<IRepositoryManager>();
        _mockRepositoryManager.Setup(x => x.ServerRepository).Returns(mockServerRepository.Object);
        _mockRepositoryManager.Setup(x => x.UserRepository).Returns(mockUserRepository.Object);
        _validator = new GetUserServerChannelsQueryValidator(_mockRepositoryManager.Object);
    }

    [Fact]
    public async Task UserId_IsEmpty_ReturnsAnError()
    {
        // Arrange
        Guid userId = Guid.Empty;
        Server targetServer = GetTargetServer();
        targetServer.AddMember(userId, "user", DateTime.UtcNow);
        GetUserServerChannelsQuery query = new(userId, targetServer);
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(targetServer);
        _mockRepositoryManager.Setup(x => x.UserRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(GetUser(userId));

        // Act
        var result = await _validator.ValidateAsync(query, It.IsAny<CancellationToken>());

        // Assert
        Assert.NotEmpty(result.Errors);
        Assert.True(result.Errors?.Any(x => x.PropertyName == "UserId"));
    }

    [Fact]
    public async Task UserId_IsInvalid_ThrowsUserNotFoundException()
    {
        // Arrange
        Server targetServer = GetTargetServer();
        GetUserServerChannelsQuery query = new(Guid.NewGuid(), targetServer);
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(targetServer);
        _mockRepositoryManager.Setup(x => x.UserRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User)null!);

        // Act & Assert
        await Assert.ThrowsAsync<UserNotFoundException>(() => _validator.ValidateAsync(query, It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task TargetServer_IsInvalid_ThrowsServerNotFoundException()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        Server targetServer = GetTargetServer();
        GetUserServerChannelsQuery query = new(userId, targetServer);
        _mockRepositoryManager.Setup(x => x.UserRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(GetUser(userId));
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Server)null!);

        // Act & Assert
        await Assert.ThrowsAsync<ServerNotFoundException>(() => _validator.ValidateAsync(query, It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task User_IsNotAMemberOfTheServer_ThrowsUserIsNotAMemberException()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        Server targetServer = GetTargetServer();
        GetUserServerChannelsQuery query = new(userId, targetServer);
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(targetServer);
        _mockRepositoryManager.Setup(x => x.UserRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(GetUser(userId));

        // Act & Assert
        await Assert.ThrowsAsync<UserIsNotAMemberException>(() => _validator.ValidateAsync(query, It.IsAny<CancellationToken>()));
    }

    private static Server GetTargetServer()
    {
        Server targetServer = new("Target Server", "Short Desc", "Long Desc", "creator@example.com", "");
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
