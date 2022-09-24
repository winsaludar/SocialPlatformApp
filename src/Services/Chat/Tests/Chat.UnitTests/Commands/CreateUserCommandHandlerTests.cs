using Chat.Application.Commands;
using Chat.Domain.Aggregates.UserAggregate;
using Chat.Domain.SeedWork;
using Moq;

namespace Chat.UnitTests.Commands;

public class CreateUserCommandHandlerTests
{
    private readonly Mock<IRepositoryManager> _mockRepositoryManager;
    private readonly CreateUserCommandHandler _createUserCommandHandler;

    public CreateUserCommandHandlerTests()
    {
        Mock<IUserRepository> mockUserRepository = new();
        _mockRepositoryManager = new Mock<IRepositoryManager>();
        _mockRepositoryManager.Setup(x => x.UserRepository).Returns(mockUserRepository.Object);
        _createUserCommandHandler = new(_mockRepositoryManager.Object);
    }

    [Fact]
    public async Task Handle_UserCreated_ReturnsUserGuid()
    {
        // Arrange
        CreateUserCommand command = new(Guid.NewGuid(), "Username", "user@example.com", Guid.NewGuid());
        _mockRepositoryManager.Setup(x => x.UserRepository.AddAsync(It.IsAny<User>()))
            .ReturnsAsync(Guid.NewGuid());

        // Act
        var result = await _createUserCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // Assert
        Assert.NotEqual(Guid.Empty, result);
    }
}
