using Moq;
using Space.Domain.Entities;
using Space.Domain.Exceptions;
using Space.Domain.Helpers;
using Space.Domain.Repositories;
using DomainEntities = Space.Domain.Entities;

namespace Space.UnitTests.Entities;

public class SpaceTests
{
    private readonly Mock<IRepositoryManager> _mockRepo;
    private readonly Mock<IHelperManager> _mockHelper;
    private readonly Mock<ISlugHelper> _mockSlugHelper;

    public SpaceTests()
    {
        Mock<IUnitOfWork> mockUnitOfWork = new();
        Mock<ISpaceRepository> mockSpaceRepo = new();
        Mock<ISoulRepository> mockSoulRepo = new();
        _mockRepo = new Mock<IRepositoryManager>();
        _mockRepo.Setup(x => x.UnitOfWork).Returns(mockUnitOfWork.Object);
        _mockRepo.Setup(x => x.SpaceRepository).Returns(mockSpaceRepo.Object);
        _mockRepo.Setup(x => x.SoulRepository).Returns(mockSoulRepo.Object);

        _mockSlugHelper = new Mock<ISlugHelper>();
        _mockHelper = new Mock<IHelperManager>();
        _mockHelper.Setup(x => x.SlugHelper).Returns(_mockSlugHelper.Object);
    }

    [Fact]
    public async Task KickSoulAsync_RepositoryManagerIsNull_ThrowsArgumentNullException()
    {
        DomainEntities.Space space = new() { };
        string email = "member@example.com";

        await Assert.ThrowsAsync<ArgumentNullException>(() => space.KickSoulAsync(email));
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task KickSoulAsync_EmailIsInvalid_ThrowsInvalidSoulException(string email)
    {
        DomainEntities.Space space = new(_mockRepo.Object) { };

        await Assert.ThrowsAsync<InvalidSoulException>(() => space.KickSoulAsync(email));
    }

    [Fact]
    public async Task KickSoulAsync_SpaceIsInvalid_ThrowsInvalidSpaceIdException()
    {
        DomainEntities.Space space = new(_mockRepo.Object) { Id = Guid.NewGuid() };
        string email = "member@example.com";

        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
            .ReturnsAsync((DomainEntities.Space)null!);

        await Assert.ThrowsAsync<InvalidSpaceIdException>(() => space.KickSoulAsync(email));
    }

    [Fact]
    public async Task KickSoulAsync_SoulDoesNotExist_ThrowsInvalidSoulException()
    {
        DomainEntities.Space space = new(_mockRepo.Object) { Id = Guid.NewGuid() };
        string email = "notexisting@example.com";

        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
            .ReturnsAsync(new DomainEntities.Space());
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync((Soul)null!);

        await Assert.ThrowsAsync<InvalidSoulException>(() => space.KickSoulAsync(email));
    }

    [Fact]
    public async Task KickSoulAsync_SoulIsNotAMemberOfTheSpace_ThrowsSoulNotMemberException()
    {
        DomainEntities.Space space = new(_mockRepo.Object) { Id = Guid.NewGuid() };
        string email = "notmember@example.com";

        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
            .ReturnsAsync(new DomainEntities.Space());
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(new Soul());
        _mockRepo.Setup(x => x.SoulRepository.IsMemberOfSpaceAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(false);

        await Assert.ThrowsAsync<SoulNotMemberException>(() => space.KickSoulAsync(email));
    }

    [Fact]
    public async Task KickSoulAsync_SoulAndSpaceAreBothValidAndSoulIsAMember_RemoveToSpace()
    {
        DomainEntities.Space space = new(_mockRepo.Object) { Id = Guid.NewGuid() };
        Soul existingSoul = new()
        {
            Id = Guid.NewGuid(),
            Email = "existingg@example.com",
            Name = "existing@example.com",
            CreatedBy = "existing@example.com",
            CreatedDateUtc = DateTime.UtcNow
        };

        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
            .ReturnsAsync(space);
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(existingSoul);
        _mockRepo.Setup(x => x.SoulRepository.IsMemberOfSpaceAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(true);

        await space.KickSoulAsync(existingSoul.Email);

        _mockRepo.Verify(x => x.SoulRepository.DeleteSoulSpaceAsync(existingSoul.Id, space.Id), Times.Once);
        _mockRepo.Verify(x => x.UnitOfWork.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateTopicAsync_RepositoryManagerIsNull_ThrowsArgumentNullException()
    {
        DomainEntities.Space space = new() { };

        await Assert.ThrowsAsync<ArgumentNullException>(() => space.CreateTopicAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
    }

    [Fact]
    public async Task CreateTopicAsync_HelperManagerIsNull_ThrowsArgumentNullException()
    {
        DomainEntities.Space space = new(_mockRepo.Object) { };

        await Assert.ThrowsAsync<ArgumentNullException>(() => space.CreateTopicAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task CreateTopicAsync_AuthorEmailIsInvalid_ThrowsInvalidSoulException(string authorEmail)
    {
        DomainEntities.Space space = new(_mockRepo.Object, _mockHelper.Object) { };
        string title = "Fake Title";
        string content = "Fake Content";

        await Assert.ThrowsAsync<InvalidSoulException>(() => space.CreateTopicAsync(authorEmail, title, content));
    }

    [Fact]
    public async Task CreateTopicAsync_SpaceIsInvalid_ThrowsInvalidSpaceIdException()
    {
        DomainEntities.Space space = new(_mockRepo.Object, _mockHelper.Object) { Id = Guid.NewGuid() };
        string authorEmail = "author@example.com";
        string title = "Fake Title";
        string content = "Fake Content";

        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
            .ReturnsAsync((DomainEntities.Space)null!);

        await Assert.ThrowsAsync<InvalidSpaceIdException>(() => space.CreateTopicAsync(authorEmail, title, content));
    }

    [Fact]
    public async Task CreateTopicAsync_SoulDoesNotExist_ThrowsInvalidSoulException()
    {
        DomainEntities.Space space = new(_mockRepo.Object, _mockHelper.Object) { Id = Guid.NewGuid() };
        string authorEmail = "notexisting@example.com";
        string title = "Fake Title";
        string content = "Fake Content";

        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
            .ReturnsAsync(new DomainEntities.Space());
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync((Soul)null!);

        await Assert.ThrowsAsync<InvalidSoulException>(() => space.CreateTopicAsync(authorEmail, title, content));
    }

    [Fact]
    public async Task CreateTopicAsync_SoulIsNotAMemberOfTheSpace_ThrowsSoulNotMemberException()
    {
        DomainEntities.Space space = new(_mockRepo.Object, _mockHelper.Object) { Id = Guid.NewGuid() };
        string authorEmail = "notmember@example.com";
        string title = "Fake Title";
        string content = "Fake Content";

        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
            .ReturnsAsync(new DomainEntities.Space());
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(new Soul());
        _mockRepo.Setup(x => x.SoulRepository.IsMemberOfSpaceAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(false);

        await Assert.ThrowsAsync<SoulNotMemberException>(() => space.CreateTopicAsync(authorEmail, title, content));
    }

    [Fact]
    public async Task CreateTopicAsync_SoulAndSpaceAreBothValidAndSoulIsAMember_CreateTopic()
    {
        Topic? createdTopic = null;
        DomainEntities.Space space = new(_mockRepo.Object, _mockHelper.Object) { Id = Guid.NewGuid() };
        string authorEmail = "member@example.com";
        Topic newTopic = new(_mockHelper.Object)
        {
            Title = "Fake Title",
            Content = "Fake Content",
            SpaceId = space.Id,
            SoulId = Guid.NewGuid(),
            CreatedBy = authorEmail,
            CreatedDateUtc = DateTime.UtcNow
        };

        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
            .ReturnsAsync(new DomainEntities.Space());
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(new Soul());
        _mockRepo.Setup(x => x.SoulRepository.IsMemberOfSpaceAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(true);
        _mockSlugHelper.Setup(x => x.CreateSlug(It.IsAny<string>()))
            .Returns("fake-title");
        _mockRepo.Setup(x => x.SpaceRepository.CreateTopicAsync(It.IsAny<Topic>()))
            .Callback<Topic>(x => createdTopic = new Topic
            {
                Id = Guid.NewGuid(),
                Content = newTopic.Content,
                SpaceId = newTopic.SpaceId,
                SoulId = newTopic.SoulId,
                CreatedBy = newTopic.CreatedBy,
                CreatedDateUtc = newTopic.CreatedDateUtc
            });

        await space.CreateTopicAsync(authorEmail, newTopic.Title, newTopic.Content);

        _mockRepo.Verify(x => x.SpaceRepository.CreateTopicAsync(It.IsAny<Topic>()), Times.Once);
        Assert.NotNull(createdTopic);
        Assert.NotEqual(Guid.Empty, createdTopic?.Id);
    }
}
