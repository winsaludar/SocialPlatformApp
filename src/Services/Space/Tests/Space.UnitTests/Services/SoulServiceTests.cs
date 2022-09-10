using Moq;
using Space.Contracts;
using Space.Domain.Entities;
using Space.Domain.Helpers;
using Space.Domain.Repositories;
using Space.Services;
using Space.Services.Abstraction;

namespace Space.UnitTests.Services;

public class SoulServiceTests
{
    private readonly Mock<IRepositoryManager> _mockRepo;
    private readonly Mock<IHelperManager> _mockHelper;
    private readonly ISoulService _soulService;

    public SoulServiceTests()
    {
        _mockRepo = new Mock<IRepositoryManager>();
        _mockHelper = new Mock<IHelperManager>();

        Mock<ISpaceRepository> mockSpaceRepo = new();
        Mock<ISoulRepository> mockSoulRepo = new();
        _mockRepo.Setup(x => x.SpaceRepository).Returns(mockSpaceRepo.Object);
        _mockRepo.Setup(x => x.SoulRepository).Returns(mockSoulRepo.Object);

        _soulService = new SoulService(_mockRepo.Object, _mockHelper.Object);
    }

    [Fact]
    public async Task GetAllTopicsByIdAsync_SoulIsNull_ReturnsEmptyTopicsDto()
    {
        Guid soulId = Guid.NewGuid();

        _mockRepo.Setup(x => x.SoulRepository.GetByIdAsync(soulId, It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync((Soul)null!);

        var result = await _soulService.GetAllTopicsByIdAsync(soulId);

        _mockRepo.Verify(x => x.SpaceRepository.GetTopicVotesAsync(It.IsAny<Guid>()), Times.Never());
        Assert.IsType<List<TopicDto>>(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllTopicsByIdAsync_SoulIsNotNull_ReturnsNonEmptyTopicsDto()
    {
        Guid soulId = Guid.NewGuid();
        var mockVotesOutput = (0, 0);

        _mockRepo.Setup(x => x.SoulRepository.GetByIdAsync(soulId, It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new Soul
            {
                Topics = new List<Topic>
                {
                    new Topic(),
                    new Topic(),
                    new Topic()
                }
            });
        _mockRepo.Setup(x => x.SpaceRepository.GetTopicVotesAsync(It.IsAny<Guid>()))
            .ReturnsAsync(mockVotesOutput);

        var result = await _soulService.GetAllTopicsByIdAsync(soulId);

        _mockRepo.Verify(x => x.SpaceRepository.GetTopicVotesAsync(It.IsAny<Guid>()), Times.AtLeastOnce());
        Assert.IsType<List<TopicDto>>(result);
        Assert.NotEmpty(result);
        Assert.Equal(3, result.Count());
    }

    [Fact]
    public async Task GetAllTopicsByEmailAsync_SoulIsNull_ReturnsEmptyTopicsDto()
    {
        string email = "user@example.com";

        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(email, It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync((Soul)null!);

        var result = await _soulService.GetAllTopicsByEmailAsync(email);

        _mockRepo.Verify(x => x.SpaceRepository.GetTopicVotesAsync(It.IsAny<Guid>()), Times.Never());
        Assert.IsType<List<TopicDto>>(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllTopicsByEmailAsync_SoulIsNotNull_ReturnsNonEmptyTopicsDto()
    {
        string email = "user@example.com";
        var mockVotesOutput = (0, 0);

        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(email, It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new Soul
            {
                Topics = new List<Topic>
                {
                    new Topic(),
                    new Topic(),
                    new Topic()
                }
            });
        _mockRepo.Setup(x => x.SpaceRepository.GetTopicVotesAsync(It.IsAny<Guid>()))
            .ReturnsAsync(mockVotesOutput);

        var result = await _soulService.GetAllTopicsByEmailAsync(email);

        _mockRepo.Verify(x => x.SpaceRepository.GetTopicVotesAsync(It.IsAny<Guid>()), Times.AtLeastOnce());
        Assert.IsType<List<TopicDto>>(result);
        Assert.NotEmpty(result);
        Assert.Equal(3, result.Count());
    }

    [Fact]
    public async Task GetAllModeratedSpacesAsync_SoulIsNull_ReturnsEmptySpacesDto()
    {
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync((Soul)null!);

        var result = await _soulService.GetAllModeratedSpacesAsync(It.IsAny<string>());

        Assert.IsType<List<SpaceDto>>(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllModeratedSpacesAsync_SoulIsNotNull_ReturnsNonEmptySpacesDto()
    {
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new Soul
            {
                SpacesAsModerator = new List<Domain.Entities.Space>
                {
                    new Domain.Entities.Space(),
                    new Domain.Entities.Space(),
                    new Domain.Entities.Space()
                }
            });

        var result = await _soulService.GetAllModeratedSpacesAsync(It.IsAny<string>());

        Assert.IsType<List<SpaceDto>>(result);
        Assert.NotEmpty(result);
        Assert.Equal(3, result.Count());
    }
}
