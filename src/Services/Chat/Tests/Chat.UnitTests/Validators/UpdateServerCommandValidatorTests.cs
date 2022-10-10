using Chat.Application.Commands;
using Chat.Application.Validators;
using Chat.Domain.Aggregates.ServerAggregate;
using Chat.Domain.Exceptions;
using Chat.Domain.SeedWork;
using Moq;

namespace Chat.UnitTests.Validators;

public class UpdateServerCommandValidatorTests
{
    private readonly Mock<IRepositoryManager> _mockRepositoryManager;
    private readonly UpdateServerCommandValidator _validator;

    public UpdateServerCommandValidatorTests()
    {
        Mock<IServerRepository> mockServerRepository = new();
        _mockRepositoryManager = new Mock<IRepositoryManager>();
        _mockRepositoryManager.Setup(x => x.ServerRepository).Returns(mockServerRepository.Object);
        _validator = new UpdateServerCommandValidator(_mockRepositoryManager.Object);
    }

    [Fact]
    public async Task TargetServerId_IsEmpty_ReturnsError()
    {
        // Arrange
        UpdateServerCommand command = new(Guid.Empty, "Name", "Short Description", "Long Description", "user@example.com", "Thumbnail");
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(
            new Server(command.Name, command.ShortDescription, command.LongDescription, command.EditorEmail, command.Thumbnail));

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
        UpdateServerCommand command = new(Guid.NewGuid(), "Name", "Short Description", "Long Description", "user@example.com", "Thumbnail");
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Server)null!);

        // Act & Assert
        await Assert.ThrowsAsync<ServerNotFoundException>(() => _validator.ValidateAsync(command, It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task Name_IsEmpty_ReturnsError()
    {
        // Arrange
        UpdateServerCommand command = new(Guid.NewGuid(), "", "Short Description", "Long Description", "user@example.com", "Thumbnail");
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(
            new Server(command.Name, command.ShortDescription, command.LongDescription, command.EditorEmail, command.Thumbnail));

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
        string name = "This is a long server name that exceeds 50 characters in length 1234567890.";
        UpdateServerCommand command = new(Guid.NewGuid(), name, "Short Description", "Long Description", "user@example.com", "Thumbnail");
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(
            new Server(command.Name, command.ShortDescription, command.LongDescription, command.EditorEmail, command.Thumbnail));

        // Act
        var result = await _validator.ValidateAsync(command, It.IsAny<CancellationToken>());

        // Assert
        Assert.NotEmpty(result.Errors);
        Assert.True(result.Errors?.Any(x => x.PropertyName == "Name"));
    }

    [Fact]
    public async Task Name_NewNameAlreadyExist_ReturnsError()
    {
        // Arrange
        UpdateServerCommand command = new(Guid.NewGuid(), "New Name", "Short Description", "Long Description", "user@example.com", "Thumbnail");
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(
            new Server(command.Name, command.ShortDescription, command.LongDescription, command.EditorEmail, command.Thumbnail));
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByNameAsync(It.IsAny<string>())).ReturnsAsync(
            new Server("Existing Name", "Existing Short Desc", "Existing Long Desc", "user@example.com", ""));

        // Act & Assert
        await Assert.ThrowsAsync<ServerNameAlreadyExistException>(() => _validator.ValidateAsync(command, It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task ShortDescription_IsEmpty_ReturnsAnError()
    {
        // Arrange
        UpdateServerCommand command = new(Guid.NewGuid(), "Name", "", "Long Description", "user@example.com", "Thumbnail");
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(
            new Server(command.Name, command.ShortDescription, command.LongDescription, command.EditorEmail, command.Thumbnail));

        // Act
        var result = await _validator.ValidateAsync(command, It.IsAny<CancellationToken>());

        // Assert
        Assert.NotEmpty(result.Errors);
        Assert.True(result.Errors?.Any(x => x.PropertyName == "ShortDescription"));
    }

    [Fact]
    public async Task ShortDescription_ExceedsMaximumCharacterLength_ReturnsAnError()
    {
        // Arrange
        string shortDescription = @"This is a long server short description that exceeds 200 characters in length 1234567890. 
        This is a long server short description that exceeds 200 characters in length 1234567890. This is a long server short description that exceeds 200 characters 
        in length 1234567890. This is a long server short description that exceeds 200 characters in length 1234567890.";
        UpdateServerCommand command = new(Guid.NewGuid(), "Name", shortDescription, "Long Description", "user@example.com", "Thumbnail");
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(
            new Server(command.Name, command.ShortDescription, command.LongDescription, command.EditorEmail, command.Thumbnail));

        // Act
        var result = await _validator.ValidateAsync(command, It.IsAny<CancellationToken>());

        // Assert
        Assert.NotEmpty(result.Errors);
        Assert.True(result.Errors?.Any(x => x.PropertyName == "ShortDescription"));
    }

    [Fact]
    public async Task LongDescription_IsEmpty_ReturnsAnError()
    {
        // Arrange
        UpdateServerCommand command = new(Guid.NewGuid(), "Name", "Short Description", "", "user@example.com", "Thumbnail");
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(
            new Server(command.Name, command.ShortDescription, command.LongDescription, command.EditorEmail, command.Thumbnail));

        // Act
        var result = await _validator.ValidateAsync(command, It.IsAny<CancellationToken>());

        // Assert
        Assert.NotEmpty(result.Errors);
        Assert.True(result.Errors?.Any(x => x.PropertyName == "LongDescription"));
    }

    [Fact]
    public async Task EditorEmail_IsEmpty_ReturnsAnError()
    {
        // Arrange
        UpdateServerCommand command = new(Guid.NewGuid(), "Name", "Short Description", "Long Description", "", "Thumbnail");
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(
            new Server(command.Name, command.ShortDescription, command.LongDescription, command.EditorEmail, command.Thumbnail));

        // Act
        var result = await _validator.ValidateAsync(command, It.IsAny<CancellationToken>());

        // Assert
        Assert.NotEmpty(result.Errors);
        Assert.True(result.Errors?.Any(x => x.PropertyName == "EditorEmail"));
    }

    [Fact]
    public async Task EditorEmail_IsNotValidEmailAddress_ReturnsAnError()
    {
        // Arrange
        UpdateServerCommand command = new(Guid.NewGuid(), "Name", "Short Description", "Long Description", "notvalidemail", "Thumbnail");
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(
            new Server(command.Name, command.ShortDescription, command.LongDescription, command.EditorEmail, command.Thumbnail));

        // Act
        var result = await _validator.ValidateAsync(command, It.IsAny<CancellationToken>());

        // Assert
        Assert.NotEmpty(result.Errors);
        Assert.True(result.Errors?.Any(x => x.PropertyName == "EditorEmail"));
    }

    [Fact]
    public async Task EditorEmail_NotTheSameWithCreatorEmail_ReturnsAnError()
    {
        // Arrange
        UpdateServerCommand command = new(Guid.NewGuid(), "Name", "Short Description", "Long Description", "user1@example.com", "Thumbnail");
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(
            new Server(command.Name, command.ShortDescription, command.LongDescription, "user2@example.com", command.Thumbnail));

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedServerEditorException>(() => _validator.ValidateAsync(command, It.IsAny<CancellationToken>()));
    }
}
