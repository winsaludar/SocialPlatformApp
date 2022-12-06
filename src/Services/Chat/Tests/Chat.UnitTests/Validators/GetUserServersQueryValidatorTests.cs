using Chat.Application.Queries;
using Chat.Application.Validators;
using Chat.Domain.Aggregates.UserAggregate;
using Chat.Domain.Exceptions;
using Chat.Domain.SeedWork;
using Moq;

namespace Chat.UnitTests.Validators;

public class GetUserServersQueryValidatorTests
{
    private readonly Mock<IRepositoryManager> _mockRepositoryManager;
    private readonly GetUserServersQueryValidator _validator;

    public GetUserServersQueryValidatorTests()
    {
        Mock<IUserRepository> mockUserRepository = new();
        _mockRepositoryManager = new Mock<IRepositoryManager>();
        _mockRepositoryManager.Setup(x => x.UserRepository).Returns(mockUserRepository.Object);
        _validator = new GetUserServersQueryValidator(_mockRepositoryManager.Object);
    }

    [Fact]
    public async Task UserId_IsEmpty_ReturnsAnError()
    {
        // Arrange
        GetUserServersQuery query = new(Guid.Empty);
        _mockRepositoryManager.Setup(x => x.UserRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(GetUser(Guid.Empty));

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
        GetUserServersQuery query = new(Guid.NewGuid());
        _mockRepositoryManager.Setup(x => x.UserRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User)null!);

        // Act & Assert
        await Assert.ThrowsAsync<UserNotFoundException>(() => _validator.ValidateAsync(query, It.IsAny<CancellationToken>()));
    }

    private static User GetUser(Guid id)
    {
        User user = new(Guid.NewGuid(), "user", "user@example.com");
        user.SetId(id);

        return user;
    }
}
