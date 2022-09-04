using Moq;
using Space.Contracts;
using Space.Domain.Entities;
using Space.Domain.Repositories;
using Space.Services;
using Space.Services.Abstraction;

namespace Space.UnitTests.Services;

public class SoulServiceTests
{
    private readonly Mock<IRepositoryManager> _mockRepo;
    private readonly ISoulService _soulService;

    public SoulServiceTests()
    {
        Mock<ISpaceRepository> mockSpaceRepo = new();
        Mock<ISoulRepository> mockSoulRepo = new();
        _mockRepo = new Mock<IRepositoryManager>();
        _mockRepo.Setup(x => x.SpaceRepository).Returns(mockSpaceRepo.Object);
        _mockRepo.Setup(x => x.SoulRepository).Returns(mockSoulRepo.Object);

        _soulService = new SoulService(_mockRepo.Object);
    }

    [Fact]
    public async Task GetAllTopicsAsync_SoulIsNull_ReturnsEmptyTopicsDto()
    {
        Guid soulId = Guid.NewGuid();

        _mockRepo.Setup(x => x.SoulRepository.GetByIdAsync(soulId, It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync((Soul)null!);

        var result = await _soulService.GetAllTopicsAsync(soulId);

        Assert.IsType<List<TopicDto>>(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllTopicsAsync_SpaceIsNotNull_ReturnsNotEmptyTopicsDto()
    {
        Guid soulId = Guid.NewGuid();

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

        var result = await _soulService.GetAllTopicsAsync(soulId);

        Assert.IsType<List<TopicDto>>(result);
        Assert.NotEmpty(result);
        Assert.Equal(3, result.Count());
    }
}
