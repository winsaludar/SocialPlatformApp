using Chat.Application.Commands;
using Chat.Application.Validators;
using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.Exceptions;
using Chat.Domain.SeedWork;
using Moq;

namespace Chat.UnitTests.Validators;

public class CreateServerCommandValidatorTests
{
    private readonly Mock<IRepositoryManager> _mockRepositoryManager;
    private readonly CreateServerCommandValidator _validator;

    public CreateServerCommandValidatorTests()
    {
        Mock<IServerRepository> mockServerRepository = new();
        _mockRepositoryManager = new Mock<IRepositoryManager>();
        _mockRepositoryManager.Setup(x => x.ServerRepository).Returns(mockServerRepository.Object);
        _validator = new CreateServerCommandValidator(_mockRepositoryManager.Object);
    }

    [Fact]
    public async Task Name_IsEmpty_ReturnAnError()
    {
        // Arrange
        CreateServerCommand command = new("", "Short Description", "Long Description", "Thumbnail");

        // Act
        var result = await _validator.ValidateAsync(command, It.IsAny<CancellationToken>());

        // Assert
        Assert.NotEmpty(result.Errors);
        Assert.True(result.Errors?.Any(x => x.PropertyName == "Name"));
    }

    [Fact]
    public async Task Name_ExceedsMaximumCharacterLength_ReturnAnError()
    {
        // Arrange
        string name = "This is a long server name that exceeds 50 characters in length 1234567890.";
        CreateServerCommand command = new(name, "Short Description", "Long Description", "Thumbnail");

        // Act
        var result = await _validator.ValidateAsync(command, It.IsAny<CancellationToken>());

        // Assert
        Assert.NotEmpty(result.Errors);
        Assert.True(result.Errors?.Any(x => x.PropertyName == "Name"));
    }

    [Fact]
    public async Task Name_AlreadyExist_ThrowsNameAlreadyInUseException()
    {
        // Arrange
        string name = "Existing Name";
        CreateServerCommand command = new(name, "Short Description", "Long Description", "Thumbnail");
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(new Server(command.Name, command.ShortDescription, command.LongDescription, command.Thumbnail));

        // Act & Assert
        await Assert.ThrowsAsync<NameAlreadyInUseException>(() => _validator.ValidateAsync(command, It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task ShortDescription_IsEmpty_ReturnAnError()
    {
        // Arrange
        CreateServerCommand command = new("Name", "", "Long Description", "Thumbnail");

        // Act
        var result = await _validator.ValidateAsync(command, It.IsAny<CancellationToken>());

        // Assert
        Assert.NotEmpty(result.Errors);
        Assert.True(result.Errors?.Any(x => x.PropertyName == "ShortDescription"));
    }

    [Fact]
    public async Task ShortDescription_ExceedsMaximumCharacterLength_ReturnAnError()
    {
        // Arrange
        string shortDescription = @"This is a long server short description that exceeds 200 characters in length 1234567890. 
        This is a long server short description that exceeds 200 characters in length 1234567890. This is a long server short description that exceeds 200 characters 
        in length 1234567890. This is a long server short description that exceeds 200 characters in length 1234567890.";
        CreateServerCommand command = new("Name", shortDescription, "Long Description", "Thumbnail");

        // Act
        var result = await _validator.ValidateAsync(command, It.IsAny<CancellationToken>());

        // Assert
        Assert.NotEmpty(result.Errors);
        Assert.True(result.Errors?.Any(x => x.PropertyName == "ShortDescription"));
    }

    [Fact]
    public async Task LongDescription_IsEmpty_ReturnAnError()
    {
        // Arrange
        CreateServerCommand command = new("Name", "Short Description", "", "Thumbnail");

        // Act
        var result = await _validator.ValidateAsync(command, It.IsAny<CancellationToken>());

        // Assert
        Assert.NotEmpty(result.Errors);
        Assert.True(result.Errors?.Any(x => x.PropertyName == "LongDescription"));
    }
}
