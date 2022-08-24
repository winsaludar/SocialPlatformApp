using Moq;
using Space.Contracts;
using Space.Domain.Exceptions;
using Space.Domain.Repositories;
using Space.Services;
using Space.Services.Abstraction;
using DomainEntities = Space.Domain.Entities;

namespace Space.UnitTests.Services;

public class SpaceServiceTests
{
    private readonly Mock<IRepositoryManager> _mockRepo;
    private readonly ISpaceService _spaceService;

    public SpaceServiceTests()
    {
        _mockRepo = new Mock<IRepositoryManager>();
        Mock<ISpaceRepository> mockSpaceRepo = new();
        _mockRepo.Setup(x => x.SpaceRepository).Returns(mockSpaceRepo.Object);
        _spaceService = new SpaceService(_mockRepo.Object);
    }

    [Fact]
    public async Task GetAllAsync_SpacesAreEmptyFromDatabase_ReturnsEmptySpacesDto()
    {
        _mockRepo.Setup(x => x.SpaceRepository.GetAllAsync())
            .ReturnsAsync((IEnumerable<DomainEntities.Space>)null!);

        var result = await _spaceService.GetAllAsync();

        Assert.IsType<List<SpaceDto>>(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_SpacesAreNotEmptyFromDatabase_ReturnsSpacesDto()
    {
        _mockRepo.Setup(x => x.SpaceRepository.GetAllAsync())
            .ReturnsAsync(new List<DomainEntities.Space>()
            {
                new DomainEntities.Space { },
                new DomainEntities.Space { },
                new DomainEntities.Space { }
            });

        var result = await _spaceService.GetAllAsync();

        Assert.IsType<List<SpaceDto>>(result);
        Assert.NotEmpty(result);
        Assert.Equal(3, result.Count());
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task CreateAsync_SpaceNameIsInvalid_ThrowsInvalidSpaceNameException(string name)
    {
        SpaceDto space = new() { Name = name };

        await Assert.ThrowsAsync<InvalidSpaceNameException>(() => _spaceService.CreateAsync(space));
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task CreateAsync_SpaceCreatorIsInvalid_ThrowsInvalidSpaceCreatorException(string creator)
    {
        SpaceDto space = new() { Name = "Test", Creator = creator };

        await Assert.ThrowsAsync<InvalidSpaceCreatorException>(() => _spaceService.CreateAsync(space));
    }

    [Fact]
    public async Task CreateAsync_SpaceNameAlreadyExist_ThrowsSpaceNameAlreadyExistException()
    {
        _mockRepo.Setup(x => x.SpaceRepository.GetByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(new DomainEntities.Space());
        SpaceDto space = new() { Name = "Test", Creator = "test@example.com" };

        await Assert.ThrowsAsync<SpaceNameAlreadyExistException>(() => _spaceService.CreateAsync(space));
    }

    [Fact]
    public async Task CreateAsync_SpaceIsValid_CreateSpace()
    {
        DomainEntities.Space? createdSpace = null;
        _mockRepo.Setup(x => x.SpaceRepository.GetByNameAsync(It.IsAny<string>()))
            .ReturnsAsync((DomainEntities.Space)null!);
        _mockRepo.Setup(x => x.SpaceRepository.CreateAsync(It.IsAny<DomainEntities.Space>()))
            .Callback<DomainEntities.Space>(x => createdSpace = x);
        SpaceDto newSpace = new()
        {
            Name = "Test",
            Creator = "test@example.com",
            ShortDescription = "Short Desc",
            LongDescription = "Long Desc"
        };

        await _spaceService.CreateAsync(newSpace);

        _mockRepo.Verify(x => x.SpaceRepository.CreateAsync(It.IsAny<DomainEntities.Space>()), Times.Once);
        Assert.Equal(createdSpace?.Name, newSpace.Name);
        Assert.Equal(createdSpace?.Creator, newSpace.Creator);
        Assert.Equal(createdSpace?.ShortDescription, newSpace.ShortDescription);
        Assert.Equal(createdSpace?.LongDescription, newSpace.LongDescription);
    }
}
