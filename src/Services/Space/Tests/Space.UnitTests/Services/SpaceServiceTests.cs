using Moq;
using Space.Contracts;
using Space.Domain.Entities;
using Space.Domain.Helpers;
using Space.Domain.Repositories;
using Space.Services;
using Space.Services.Abstraction;
using DomainEntities = Space.Domain.Entities;

namespace Space.UnitTests.Services;

public class SpaceServiceTests
{
    private readonly Mock<IRepositoryManager> _mockRepo;
    private readonly Mock<IHelperManager> _mockHelper;
    private readonly ISpaceService _spaceService;

    public SpaceServiceTests()
    {
        _mockRepo = new Mock<IRepositoryManager>();
        _mockHelper = new Mock<IHelperManager>();

        Mock<ISpaceRepository> mockSpaceRepo = new();
        Mock<ISlugHelper> mockSlugHelper = new();
        _mockRepo.Setup(x => x.SpaceRepository).Returns(mockSpaceRepo.Object);
        _mockHelper.Setup(x => x.SlugHelper).Returns(mockSlugHelper.Object);

        _spaceService = new SpaceService(_mockRepo.Object, _mockHelper.Object);
    }

    [Fact]
    public async Task GetAllAsync_SpacesAreEmptyFromDatabase_ReturnsEmptySpacesDto()
    {
        _mockRepo.Setup(x => x.SpaceRepository.GetAllAsync(It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync((IEnumerable<DomainEntities.Space>)null!);

        var result = await _spaceService.GetAllAsync();

        Assert.IsType<List<SpaceDto>>(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_SpacesAreNotEmptyFromDatabase_ReturnsNotEmptySpacesDto()
    {
        _mockRepo.Setup(x => x.SpaceRepository.GetAllAsync(It.IsAny<bool>(), It.IsAny<bool>()))
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

    [Fact]
    public async Task GetAllSoulsAsync_SoulsAreEmptyFromDatabase_ReturnsEmptySoulsDto()
    {
        Guid spaceId = Guid.NewGuid();

        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(spaceId, It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new DomainEntities.Space
            {
                Souls = new List<Soul>()
            });

        var result = await _spaceService.GetAllSoulsAsync(spaceId);

        Assert.IsType<List<SoulDto>>(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllSoulsAsync_SoulsAreNotEmptyFromDatabase_ReturnsNotEmptySoulsDto()
    {
        Guid spaceId = Guid.NewGuid();

        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(spaceId, It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new DomainEntities.Space
            {
                Souls = new List<Soul>
                {
                    new Soul(),
                    new Soul(),
                    new Soul()
                }
            });

        var result = await _spaceService.GetAllSoulsAsync(spaceId);

        Assert.IsType<List<SoulDto>>(result);
        Assert.NotEmpty(result);
        Assert.Equal(3, result.Count());
    }
}
