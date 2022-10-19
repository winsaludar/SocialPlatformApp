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
    public async Task TargetServer_IsInvalid_ThrowsServerNotFoundException()
    {
        // Arrange
        Server targetServer = GetTargetServer();
        UpdateServerCommand command = new(targetServer, "Name", "Short Description", "Long Description", Guid.NewGuid(), "Thumbnail");
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Server)null!);

        // Act & Assert
        await Assert.ThrowsAsync<ServerNotFoundException>(() => _validator.ValidateAsync(command, It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task Name_IsEmpty_ReturnsError()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        Server targetServer = GetTargetServer();
        targetServer.SetCreatedById(userId);
        UpdateServerCommand command = new(targetServer, "", "Short Description", "Long Description", userId, "Thumbnail");
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
        Guid userId = Guid.NewGuid();
        Server targetServer = GetTargetServer();
        targetServer.SetCreatedById(userId);
        string name = "This is a long server name that exceeds 50 characters in length 1234567890.";
        UpdateServerCommand command = new(targetServer, name, "Short Description", "Long Description", userId, "Thumbnail");
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(targetServer);

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
        Server targetServer = GetTargetServer();
        UpdateServerCommand command = new(targetServer, "New Name", "Short Description", "Long Description", Guid.NewGuid(), "Thumbnail");
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(GetTargetServer());
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByNameAsync(It.IsAny<string>())).ReturnsAsync(
            new Server("Existing Name", "Existing Short Desc", "Existing Long Desc", "user@example.com", ""));

        // Act & Assert
        await Assert.ThrowsAsync<ServerNameAlreadyExistException>(() => _validator.ValidateAsync(command, It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task ShortDescription_IsEmpty_ReturnsAnError()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        Server targetServer = GetTargetServer();
        targetServer.SetCreatedById(userId);
        UpdateServerCommand command = new(targetServer, "Name", "", "Long Description", userId, "Thumbnail");
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(targetServer);

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
        Guid userId = Guid.NewGuid();
        Server targetServer = GetTargetServer();
        targetServer.SetCreatedById(userId);
        string shortDescription = @"This is a long server short description that exceeds 200 characters in length 1234567890. 
        This is a long server short description that exceeds 200 characters in length 1234567890. This is a long server short description that exceeds 200 characters 
        in length 1234567890. This is a long server short description that exceeds 200 characters in length 1234567890.";
        UpdateServerCommand command = new(targetServer, "Name", shortDescription, "Long Description", userId, "Thumbnail");
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(targetServer);

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
        Guid userId = Guid.NewGuid();
        Server targetServer = GetTargetServer();
        targetServer.SetCreatedById(userId);
        UpdateServerCommand command = new(targetServer, "Name", "Short Description", "", userId, "Thumbnail");
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(targetServer);

        // Act
        var result = await _validator.ValidateAsync(command, It.IsAny<CancellationToken>());

        // Assert
        Assert.NotEmpty(result.Errors);
        Assert.True(result.Errors?.Any(x => x.PropertyName == "LongDescription"));
    }

    [Fact]
    public async Task UpdatedById_IsEmpty_ReturnsAnError()
    {
        // Arrange
        Server targetServer = GetTargetServer();
        UpdateServerCommand command = new(targetServer, "Name", "Short Description", "Long Description", Guid.Empty, "Thumbnail");
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(GetTargetServer());

        // Act
        var result = await _validator.ValidateAsync(command, It.IsAny<CancellationToken>());

        // Assert
        Assert.NotEmpty(result.Errors);
        Assert.True(result.Errors?.Any(x => x.PropertyName == "UpdatedById"));
    }

    [Fact]
    public async Task UpdatedById_NotTheSameWithCreator_ThrowsUnauthorizedUserException()
    {
        // Arrange
        Server targetServer = GetTargetServer();
        UpdateServerCommand command = new(targetServer, "Name", "Short Description", "Long Description", Guid.NewGuid(), "Thumbnail");
        _mockRepositoryManager.Setup(x => x.ServerRepository.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(GetTargetServer());

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
