using Chat.Application.Queries;
using Chat.Domain.Aggregates.UserAggregate;
using Chat.Domain.SeedWork;
using Moq;

namespace Chat.UnitTests.Queries;

public class GetUserByEmailQueryHandlerTests
{
    private readonly Mock<IRepositoryManager> _mockRepositoryManager;
    private readonly GetUserByEmailQueryHandler _getUserByEmailQueryHandler;

    public GetUserByEmailQueryHandlerTests()
    {
        Mock<IUserRepository> mockUserRepository = new();
        _mockRepositoryManager = new Mock<IRepositoryManager>();
        _mockRepositoryManager.Setup(x => x.UserRepository).Returns(mockUserRepository.Object);
        _getUserByEmailQueryHandler = new(_mockRepositoryManager.Object);
    }

    [Fact]
    public async Task Handle_RepositoryReturnsNull_ReturnsNull()
    {
        // Arrange
        GetUserByEmailQuery query = new("user@example.com");
        _mockRepositoryManager.Setup(x => x.UserRepository.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User)null!);

        // Act
        var result = await _getUserByEmailQueryHandler.Handle(query, It.IsAny<CancellationToken>());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_RepositoryReturnsUser_ReturnsUser()
    {
        // Arrange
        User user = new(Guid.NewGuid(), "user", "user@example.com");
        GetUserByEmailQuery query = new(user.Email);
        _mockRepositoryManager.Setup(x => x.UserRepository.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);

        // Act
        var result = await _getUserByEmailQueryHandler.Handle(query, It.IsAny<CancellationToken>());

        // Assert
        Assert.NotNull(result);
        Assert.IsType<User>(result);
    }
}
