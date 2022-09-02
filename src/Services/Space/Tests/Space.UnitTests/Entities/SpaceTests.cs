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
    public async Task KickMemberAsync_RepositoryManagerIsNull_ThrowsNullReferenceException()
    {
        DomainEntities.Space space = new() { };
        string email = "member@example.com";

        await Assert.ThrowsAsync<NullReferenceException>(() => space.KickMemberAsync(email));
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task KickMemberAsync_EmailIsInvalid_ThrowsInvalidSoulException(string email)
    {
        DomainEntities.Space space = new(_mockRepo.Object) { };

        await Assert.ThrowsAsync<InvalidSoulException>(() => space.KickMemberAsync(email));
    }

    [Fact]
    public async Task KickMemberAsync_SpaceIsInvalid_ThrowsInvalidSpaceIdException()
    {
        DomainEntities.Space space = new(_mockRepo.Object) { Id = Guid.NewGuid() };
        string email = "member@example.com";

        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync((DomainEntities.Space)null!);

        await Assert.ThrowsAsync<InvalidSpaceIdException>(() => space.KickMemberAsync(email));
    }

    [Fact]
    public async Task KickMemberAsync_SoulDoesNotExist_ThrowsInvalidSoulException()
    {
        DomainEntities.Space space = new(_mockRepo.Object) { Id = Guid.NewGuid() };
        string email = "notexisting@example.com";

        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new DomainEntities.Space());
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync((Soul)null!);

        await Assert.ThrowsAsync<InvalidSoulException>(() => space.KickMemberAsync(email));
    }

    [Fact]
    public async Task KickMemberAsync_SoulIsNotAMemberOfTheSpace_ThrowsSoulNotMemberException()
    {
        DomainEntities.Space space = new(_mockRepo.Object) { Id = Guid.NewGuid() };
        string email = "notmember@example.com";

        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new DomainEntities.Space());
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new Soul());
        _mockRepo.Setup(x => x.SoulRepository.IsMemberOfSpaceAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(false);

        await Assert.ThrowsAsync<SoulNotMemberException>(() => space.KickMemberAsync(email));
    }

    [Fact]
    public async Task KickMemberAsync_SoulAndSpaceAreBothValidAndSoulIsAMember_RemoveToSpace()
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

        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(space);
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(existingSoul);
        _mockRepo.Setup(x => x.SoulRepository.IsMemberOfSpaceAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(true);

        await space.KickMemberAsync(existingSoul.Email);

        _mockRepo.Verify(x => x.SoulRepository.DeleteSpaceMemberAsync(existingSoul.Id, space.Id), Times.Once);
        _mockRepo.Verify(x => x.UnitOfWork.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateTopicAsync_RepositoryManagerIsNull_ThrowsNullReferenceException()
    {
        DomainEntities.Space space = new() { };

        await Assert.ThrowsAsync<NullReferenceException>(() => space.CreateTopicAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
    }

    [Fact]
    public async Task CreateTopicAsync_HelperManagerIsNull_ThrowsNullReferenceException()
    {
        DomainEntities.Space space = new(_mockRepo.Object) { };

        await Assert.ThrowsAsync<NullReferenceException>(() => space.CreateTopicAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
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

        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>()))
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

        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new DomainEntities.Space());
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
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

        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new DomainEntities.Space());
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
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

        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new DomainEntities.Space());
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
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

    [Fact]
    public async Task UpdateTopicAsync_RepositoryManagerIsNull_ThrowsNullReferenceException()
    {
        DomainEntities.Space space = new() { };

        await Assert.ThrowsAsync<NullReferenceException>(() => space.UpdateTopicAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
    }

    [Fact]
    public async Task UpdateTopicAsync_HelperManagerIsNull_ThrowsNullReferenceException()
    {
        DomainEntities.Space space = new(_mockRepo.Object) { };

        await Assert.ThrowsAsync<NullReferenceException>(() => space.UpdateTopicAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task UpdateTopicAsync_ModifiedByIsInvalid_ThrowsInvalidSoulException(string authorEmail)
    {
        Guid topicId = Guid.NewGuid();
        string title = "Updated Title";
        string content = "Updated Content";
        DomainEntities.Space space = new(_mockRepo.Object, _mockHelper.Object) { };

        await Assert.ThrowsAsync<InvalidSoulException>(() => space.UpdateTopicAsync(topicId, authorEmail, title, content));
    }

    [Fact]
    public async Task UpdateTopicAsync_TopicIsInvalid_ThrowsInvalidTopicIdException()
    {
        Guid topicId = Guid.NewGuid();
        string modifiedBy = "author@example.com";
        string title = "Updated Title";
        string content = "Updated Content";
        DomainEntities.Space space = new(_mockRepo.Object, _mockHelper.Object) { };

        _mockRepo.Setup(x => x.SpaceRepository.GetTopicByIdAsync(topicId))
            .ReturnsAsync((Topic)null!);

        await Assert.ThrowsAsync<InvalidTopicIdException>(() => space.UpdateTopicAsync(topicId, modifiedBy, title, content));
    }

    [Fact]
    public async Task UpdateTopicAsync_SpaceIsInvalid_ThrowsInvalidSpaceIdException()
    {
        Guid topicId = Guid.NewGuid();
        string modifiedBy = "author@example.com";
        string title = "Updated Title";
        string content = "Updated Content";
        DomainEntities.Space space = new(_mockRepo.Object, _mockHelper.Object) { Id = Guid.NewGuid() };

        _mockRepo.Setup(x => x.SpaceRepository.GetTopicByIdAsync(topicId))
            .ReturnsAsync(new Topic());
        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync((DomainEntities.Space)null!);

        await Assert.ThrowsAsync<InvalidSpaceIdException>(() => space.UpdateTopicAsync(topicId, modifiedBy, title, content));
    }

    [Fact]
    public async Task UpdateTopicAsync_TopicSpaceIdAndExistingSpaceIdDoesNotMatch_ThrowsInvalidSpaceIdException()
    {
        Guid topicId = Guid.NewGuid();
        string modifiedBy = "author@example.com";
        string title = "Updated Title";
        string content = "Updated Content";
        DomainEntities.Space space = new(_mockRepo.Object, _mockHelper.Object) { Id = Guid.NewGuid() };

        _mockRepo.Setup(x => x.SpaceRepository.GetTopicByIdAsync(topicId))
            .ReturnsAsync(new Topic { SpaceId = Guid.NewGuid() });
        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new DomainEntities.Space() { Id = Guid.NewGuid() });

        await Assert.ThrowsAsync<InvalidSpaceIdException>(() => space.UpdateTopicAsync(topicId, modifiedBy, title, content));
    }

    [Fact]
    public async Task UpdateTopicAsync_SoulDoesNotExist_ThrowsInvalidSoulException()
    {
        Guid topicId = Guid.NewGuid();
        string modifiedBy = "notexisting@example.com";
        string title = "Updated Title";
        string content = "Updated Content";
        DomainEntities.Space space = new(_mockRepo.Object, _mockHelper.Object) { Id = Guid.NewGuid() };

        _mockRepo.Setup(x => x.SpaceRepository.GetTopicByIdAsync(topicId))
            .ReturnsAsync(new Topic());
        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new DomainEntities.Space());
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync((Soul)null!);

        await Assert.ThrowsAsync<InvalidSoulException>(() => space.UpdateTopicAsync(topicId, modifiedBy, title, content));
    }

    [Fact]
    public async Task UpdateTopicAsync_TopicSoulIdAndExistingSoulIdDoesNotMatch_ThrowsInvalidSoulException()
    {
        Guid topicId = Guid.NewGuid();
        string modifiedBy = "differentperson@example.com";
        string title = "Updated Title";
        string content = "Updated Content";
        DomainEntities.Space space = new(_mockRepo.Object, _mockHelper.Object) { Id = Guid.NewGuid() };

        _mockRepo.Setup(x => x.SpaceRepository.GetTopicByIdAsync(topicId))
            .ReturnsAsync(new Topic { SoulId = Guid.NewGuid() });
        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new DomainEntities.Space());
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new Soul { Id = Guid.NewGuid() });

        await Assert.ThrowsAsync<InvalidSoulException>(() => space.UpdateTopicAsync(topicId, modifiedBy, title, content));
    }

    [Fact]
    public async Task UpdateTopicAsync_SoulIsNotAMemberOfTheSpace_ThrowsSoulNotMemberException()
    {
        Guid spaceId = Guid.NewGuid();
        Guid topicId = Guid.NewGuid();
        Guid soulId = Guid.NewGuid();
        string modifiedBy = "nontmember@example.com";
        string title = "Updated Title";
        string content = "Updated Content";
        DomainEntities.Space space = new(_mockRepo.Object, _mockHelper.Object) { Id = spaceId };

        _mockRepo.Setup(x => x.SpaceRepository.GetTopicByIdAsync(topicId))
            .ReturnsAsync(new Topic { SoulId = soulId, SpaceId = spaceId });
        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new DomainEntities.Space() { Id = spaceId });
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new Soul { Id = soulId });
        _mockRepo.Setup(x => x.SoulRepository.IsMemberOfSpaceAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(false);

        await Assert.ThrowsAsync<SoulNotMemberException>(() => space.UpdateTopicAsync(topicId, modifiedBy, title, content));
    }

    [Fact]
    public async Task UpdateTopicAsync_SoulSpaceAndTopicAreAllValidAndSoulIsAMember_UpdateTopic()
    {
        Topic? updatedTopic = null;
        Guid spaceId = Guid.NewGuid();
        Guid topicId = Guid.NewGuid();
        Guid soulId = Guid.NewGuid();
        string modifiedBy = "author@example.com";
        string title = "Updated Title";
        string content = "Updated Content";
        DomainEntities.Space space = new(_mockRepo.Object, _mockHelper.Object) { Id = spaceId };

        _mockRepo.Setup(x => x.SpaceRepository.GetTopicByIdAsync(topicId))
            .ReturnsAsync(new Topic { SoulId = soulId, SpaceId = spaceId });
        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new DomainEntities.Space() { Id = spaceId });
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new Soul { Id = soulId });
        _mockRepo.Setup(x => x.SoulRepository.IsMemberOfSpaceAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(true);
        _mockRepo.Setup(x => x.SpaceRepository.UpdateTopicAsync(It.IsAny<Topic>()))
            .Callback<Topic>(x => updatedTopic = new Topic
            {
                Title = title,
                Content = content
            });

        await space.UpdateTopicAsync(topicId, modifiedBy, title, content);

        _mockRepo.Verify(x => x.SpaceRepository.UpdateTopicAsync(It.IsAny<Topic>()), Times.Once);
        Assert.NotNull(updatedTopic);
        Assert.Equal(title, updatedTopic?.Title);
        Assert.Equal(content, updatedTopic?.Content);
    }

    [Fact]
    public async Task DeleteTopicAsync_RepositoryManagerIsNull_ThrowsNullReferenceException()
    {
        DomainEntities.Space space = new() { };

        await Assert.ThrowsAsync<NullReferenceException>(() => space.DeleteTopicAsync(It.IsAny<Guid>(), It.IsAny<string>()));
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task DeleteTopicAsync_DeletedByIsInvalid_ThrowsInvalidSoulException(string deletedBy)
    {
        DomainEntities.Space space = new(_mockRepo.Object, _mockHelper.Object) { };

        await Assert.ThrowsAsync<InvalidSoulException>(() => space.DeleteTopicAsync(It.IsAny<Guid>(), deletedBy));
    }

    [Fact]
    public async Task DeleteTopicAsync_TopicIsInvalid_ThrowsInvalidTopicIdException()
    {
        Guid topicId = Guid.NewGuid();
        string deletedBy = "author@example.com";
        DomainEntities.Space space = new(_mockRepo.Object, _mockHelper.Object) { };

        _mockRepo.Setup(x => x.SpaceRepository.GetTopicByIdAsync(topicId))
            .ReturnsAsync((Topic)null!);

        await Assert.ThrowsAsync<InvalidTopicIdException>(() => space.DeleteTopicAsync(topicId, deletedBy));
    }

    [Fact]
    public async Task DeleteTopicAsync_SpaceIsInvalid_ThrowsInvalidSpaceIdException()
    {
        Guid topicId = Guid.NewGuid();
        string deletedBy = "author@example.com";
        DomainEntities.Space space = new(_mockRepo.Object, _mockHelper.Object) { Id = Guid.NewGuid() };

        _mockRepo.Setup(x => x.SpaceRepository.GetTopicByIdAsync(topicId))
            .ReturnsAsync(new Topic());
        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync((DomainEntities.Space)null!);

        await Assert.ThrowsAsync<InvalidSpaceIdException>(() => space.DeleteTopicAsync(topicId, deletedBy));
    }

    [Fact]
    public async Task DeleteTopicAsync_TopicSpaceIdAndExistingSpaceIdDoesNotMatch_ThrowsInvalidSpaceIdException()
    {
        Guid topicId = Guid.NewGuid();
        string deletedBy = "author@example.com";
        DomainEntities.Space space = new(_mockRepo.Object, _mockHelper.Object) { Id = Guid.NewGuid() };

        _mockRepo.Setup(x => x.SpaceRepository.GetTopicByIdAsync(topicId))
            .ReturnsAsync(new Topic { SpaceId = Guid.NewGuid() });
        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new DomainEntities.Space() { Id = Guid.NewGuid() });

        await Assert.ThrowsAsync<InvalidSpaceIdException>(() => space.DeleteTopicAsync(topicId, deletedBy));
    }

    [Fact]
    public async Task DeleteTopicAsync_SoulDoesNotExist_ThrowsInvalidSoulException()
    {
        Guid topicId = Guid.NewGuid();
        string deletedBy = "author@example.com";
        DomainEntities.Space space = new(_mockRepo.Object, _mockHelper.Object) { Id = Guid.NewGuid() };

        _mockRepo.Setup(x => x.SpaceRepository.GetTopicByIdAsync(topicId))
            .ReturnsAsync(new Topic());
        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new DomainEntities.Space());
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync((Soul)null!);

        await Assert.ThrowsAsync<InvalidSoulException>(() => space.DeleteTopicAsync(topicId, deletedBy));
    }

    [Fact]
    public async Task DeleteTopicAsync_TopicSoulIdAndExistingSoulIdDoesNotMatch_ThrowsInvalidSoulException()
    {
        Guid topicId = Guid.NewGuid();
        string deletedBy = "author@example.com";
        DomainEntities.Space space = new(_mockRepo.Object, _mockHelper.Object) { Id = Guid.NewGuid() };

        _mockRepo.Setup(x => x.SpaceRepository.GetTopicByIdAsync(topicId))
            .ReturnsAsync(new Topic { SoulId = Guid.NewGuid() });
        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new DomainEntities.Space());
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new Soul { Id = Guid.NewGuid() });

        await Assert.ThrowsAsync<InvalidSoulException>(() => space.DeleteTopicAsync(topicId, deletedBy));
    }

    [Fact]
    public async Task DeleteTopicAsync_SoulSpaceAndTopicAreAllValid_DeleteTopic()
    {
        Guid topicId = Guid.NewGuid();
        Guid spaceId = Guid.NewGuid();
        Guid soulId = Guid.NewGuid();
        string deletedBy = "author@example.com";
        List<Topic> topics = new()
        {
            new Topic(),
            new Topic(),
            new Topic()
        };
        DomainEntities.Space space = new(_mockRepo.Object, _mockHelper.Object) { Id = spaceId };

        _mockRepo.Setup(x => x.SpaceRepository.GetTopicByIdAsync(topicId))
            .ReturnsAsync(new Topic { SoulId = soulId, SpaceId = spaceId });
        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new DomainEntities.Space() { Id = spaceId });
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new Soul { Id = soulId });
        _mockRepo.Setup(x => x.SpaceRepository.DeleteTopicAsync(It.IsAny<Topic>()))
            .Callback<Topic>(x => topics.RemoveAt(2));

        await space.DeleteTopicAsync(topicId, deletedBy);

        _mockRepo.Verify(x => x.SpaceRepository.DeleteTopicAsync(It.IsAny<Topic>()), Times.Once);
        Assert.Equal(2, topics.Count);
    }
}
