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
        Mock<ISoulRepository> mockSoulRepo = new();
        _mockRepo.Setup(x => x.SpaceRepository).Returns(mockSpaceRepo.Object);
        _mockRepo.Setup(x => x.SoulRepository).Returns(mockSoulRepo.Object);

        Mock<ISlugHelper> mockSlugHelper = new();
        _mockHelper.Setup(x => x.SlugHelper).Returns(mockSlugHelper.Object);

        _spaceService = new SpaceService(_mockRepo.Object, _mockHelper.Object);
    }

    [Fact]
    public async Task GetAllAsync_SpacesAreEmpty_ReturnsEmptySpacesDto()
    {
        _mockRepo.Setup(x => x.SpaceRepository.GetAllAsync(It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync((IEnumerable<DomainEntities.Space>)null!);

        var result = await _spaceService.GetAllAsync();

        Assert.IsType<List<SpaceDto>>(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_SpacesAreNotEmpty_ReturnsNonEmptySpacesDto()
    {
        _mockRepo.Setup(x => x.SpaceRepository.GetAllAsync(It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
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
    public async Task GetByIdAsync_SpaceIsNull_ReturnsNull()
    {
        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync((DomainEntities.Space)null!);

        var result = await _spaceService.GetByIdAsync(It.IsAny<Guid>());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_SpaceIsNotNull_ReturnsSpaceDto()
    {
        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new DomainEntities.Space());

        var result = await _spaceService.GetByIdAsync(It.IsAny<Guid>());

        Assert.IsType<SpaceDto>(result);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetBySlugAsync_SpaceIsNull_ReturnsNull()
    {
        _mockRepo.Setup(x => x.SpaceRepository.GetBySlugAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync((DomainEntities.Space)null!);

        var result = await _spaceService.GetBySlugAsync(It.IsAny<string>());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetBySlugAsync_SpaceIsNotNull_ReturnsSpaceDto()
    {
        string slug = "sample-slug";
        _mockRepo.Setup(x => x.SpaceRepository.GetBySlugAsync(slug, It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new DomainEntities.Space());

        var result = await _spaceService.GetBySlugAsync(slug);

        Assert.IsType<SpaceDto>(result);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetAllModeratorsAsync_SpaceIsNull_ReturnsEmptySoulsDto()
    {
        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new DomainEntities.Space
            {
                Moderators = new List<Soul>()
            });

        var result = await _spaceService.GetAllModeratorsAsync(It.IsAny<Guid>());

        Assert.IsType<List<SoulDto>>(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllModeratorsAsync_SpaceIsNotNull_ReturnsNonEmptySoulsDto()
    {
        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new DomainEntities.Space
            {
                Moderators = new List<Soul>
                {
                    new Soul(),
                    new Soul(),
                    new Soul()
                }
            });

        var result = await _spaceService.GetAllModeratorsAsync(It.IsAny<Guid>());

        Assert.IsType<List<SoulDto>>(result);
        Assert.NotEmpty(result);
        Assert.Equal(3, result.Count());
    }

    [Fact]
    public async Task GetAllMembersAsync_SpaceIsNull_ReturnsEmptySoulsDto()
    {
        Guid spaceId = Guid.NewGuid();

        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(spaceId, It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new DomainEntities.Space
            {
                Members = new List<Soul>()
            });

        var result = await _spaceService.GetAllMembersAsync(spaceId);

        Assert.IsType<List<SoulDto>>(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllMembersAsync_SpaceIsNotNull_ReturnsNonEmptySoulsDto()
    {
        Guid spaceId = Guid.NewGuid();

        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(spaceId, It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new DomainEntities.Space
            {
                Members = new List<Soul>
                {
                    new Soul(),
                    new Soul(),
                    new Soul()
                }
            });

        var result = await _spaceService.GetAllMembersAsync(spaceId);

        Assert.IsType<List<SoulDto>>(result);
        Assert.NotEmpty(result);
        Assert.Equal(3, result.Count());
    }

    [Fact]
    public async Task GetAllTopicsAsync_SpaceIsNull_ReturnsEmptyTopicsDto()
    {
        Guid spaceId = Guid.NewGuid();

        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(spaceId, It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync((DomainEntities.Space)null!);

        var result = await _spaceService.GetAllTopicsAsync(spaceId);

        _mockRepo.Verify(x => x.SpaceRepository.GetTopicVotesAsync(It.IsAny<Guid>()), Times.Never());
        Assert.IsType<List<TopicDto>>(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllTopicsAsync_SpaceIsNotNull_ReturnsNonEmptyTopicsDto()
    {
        Guid spaceId = Guid.NewGuid();
        var mockVotesOutput = (0, 0);

        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(spaceId, It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new DomainEntities.Space
            {
                Topics = new List<Topic>
                {
                    new Topic(),
                    new Topic(),
                    new Topic()
                }
            });
        _mockRepo.Setup(x => x.SoulRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new Soul { Name = "Member", Email = "member@example.com" });
        _mockRepo.Setup(x => x.SpaceRepository.GetTopicVotesAsync(It.IsAny<Guid>()))
            .ReturnsAsync(mockVotesOutput);

        var result = await _spaceService.GetAllTopicsAsync(spaceId);

        _mockRepo.Verify(x => x.SpaceRepository.GetTopicVotesAsync(It.IsAny<Guid>()), Times.AtLeastOnce());
        Assert.IsType<List<TopicDto>>(result);
        Assert.NotEmpty(result);
        Assert.Equal(3, result.Count());
    }

    [Fact]
    public async Task GetTopicBySlugAsync_TopicIsNull_ReturnsNull()
    {
        _mockRepo.Setup(x => x.SpaceRepository.GetTopicBySlugAsync(It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync((Topic)null!);

        var result = await _spaceService.GetTopicBySlugAsync(It.IsAny<string>(), It.IsAny<string>());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetTopicBySlugAsync_SpaceIsNull_ReturnsNull()
    {
        _mockRepo.Setup(x => x.SpaceRepository.GetTopicBySlugAsync(It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(new Topic());
        _mockRepo.Setup(x => x.SpaceRepository.GetBySlugAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync((DomainEntities.Space)null!);

        var result = await _spaceService.GetTopicBySlugAsync(It.IsAny<string>(), It.IsAny<string>());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetTopicBySlugAsync_TopicSpaceIdAndSpaceIdDoesNotMatch_ReturnsNull()
    {
        _mockRepo.Setup(x => x.SpaceRepository.GetTopicBySlugAsync(It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(new Topic { SpaceId = Guid.NewGuid() });
        _mockRepo.Setup(x => x.SpaceRepository.GetBySlugAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new DomainEntities.Space { Id = Guid.NewGuid() });

        var result = await _spaceService.GetTopicBySlugAsync(It.IsAny<string>(), It.IsAny<string>());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetTopicBySlugAsync_AuthorIsNull_ResultHasNoEmailAndUsername()
    {
        Guid spaceId = Guid.NewGuid();
        var outputVotes = (0, 0);

        _mockRepo.Setup(x => x.SpaceRepository.GetTopicBySlugAsync(It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(new Topic { SpaceId = spaceId });
        _mockRepo.Setup(x => x.SpaceRepository.GetBySlugAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new DomainEntities.Space { Id = spaceId });
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync((Soul)null!);
        _mockRepo.Setup(x => x.SpaceRepository.GetTopicVotesAsync(It.IsAny<Guid>()))
            .ReturnsAsync(outputVotes);

        var result = await _spaceService.GetTopicBySlugAsync(It.IsAny<string>(), It.IsAny<string>());

        Assert.NotNull(result);
        Assert.Null(result?.AuthorEmail);
        Assert.Null(result?.AuthorUsername);
    }

    [Fact]
    public async Task GetTopicBySlugAsync_AuthorIsNotNull_ResultHasEmailAndUsername()
    {
        Guid spaceId = Guid.NewGuid();
        var outputVotes = (0, 0);

        _mockRepo.Setup(x => x.SpaceRepository.GetTopicBySlugAsync(It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(new Topic { SpaceId = spaceId });
        _mockRepo.Setup(x => x.SpaceRepository.GetBySlugAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new DomainEntities.Space { Id = spaceId });
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new Soul { Email = "author@example.com", Name = "author" });
        _mockRepo.Setup(x => x.SpaceRepository.GetTopicVotesAsync(It.IsAny<Guid>()))
            .ReturnsAsync(outputVotes);

        var result = await _spaceService.GetTopicBySlugAsync(It.IsAny<string>(), It.IsAny<string>());

        Assert.NotNull(result);
        Assert.Null(result?.AuthorEmail);
        Assert.Null(result?.AuthorUsername);
    }
}
