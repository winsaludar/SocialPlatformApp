using Moq;
using Space.Domain.Entities;
using Space.Domain.Exceptions;
using Space.Domain.Helpers;
using Space.Domain.Repositories;

namespace Space.UnitTests.Entities;

public class MemberSoulTests
{
    private readonly Mock<IRepositoryManager> _mockRepo;
    private readonly Mock<IHelperManager> _mockHelper;
    private readonly Mock<ISlugHelper> _mockSlugHelper;

    public MemberSoulTests()
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

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task CreateTopicAsync_EmailIsInvalid_ThrowsInvalidSoulException(string email)
    {
        Guid spaceId = Guid.NewGuid();
        MemberSoul member = new(email, spaceId, _mockRepo.Object, _mockHelper.Object) { };

        await Assert.ThrowsAsync<InvalidSoulException>(() => member.CreateTopicAsync(It.IsAny<string>(), It.IsAny<string>()));
    }

    [Fact]
    public async Task CreateTopicAsync_SpaceIsInvalid_ThrowsInvalidSpaceIdException()
    {
        string authorEmail = "author@example.com";
        Guid spaceId = Guid.NewGuid();
        MemberSoul member = new(authorEmail, spaceId, _mockRepo.Object, _mockHelper.Object) { };

        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync((Domain.Entities.Space)null!);

        await Assert.ThrowsAsync<InvalidSpaceIdException>(() => member.CreateTopicAsync(It.IsAny<string>(), It.IsAny<string>()));
    }

    [Fact]
    public async Task CreateTopicAsync_SoulDoesNotExist_ThrowsInvalidSoulException()
    {
        string authorEmail = "author@example.com";
        Guid spaceId = Guid.NewGuid();
        MemberSoul member = new(authorEmail, spaceId, _mockRepo.Object, _mockHelper.Object) { };

        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new Domain.Entities.Space());
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync((Soul)null!);

        await Assert.ThrowsAsync<InvalidSoulException>(() => member.CreateTopicAsync(It.IsAny<string>(), It.IsAny<string>()));
    }

    [Fact]
    public async Task CreateTopicAsync_SoulIsNotAMemberOfTheSpace_ThrowsSoulNotMemberException()
    {
        string authorEmail = "author@example.com";
        Guid spaceId = Guid.NewGuid();
        MemberSoul member = new(authorEmail, spaceId, _mockRepo.Object, _mockHelper.Object) { };

        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new Domain.Entities.Space());
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new Soul());
        _mockRepo.Setup(x => x.SoulRepository.IsMemberOfSpaceAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(false);

        await Assert.ThrowsAsync<SoulNotMemberException>(() => member.CreateTopicAsync(It.IsAny<string>(), It.IsAny<string>()));
    }

    [Fact]
    public async Task CreateTopicAsync_SoulAndSpaceAreBothValidAndSoulIsAMember_CreateTopic()
    {
        string authorEmail = "author@example.com";
        Guid spaceId = Guid.NewGuid();
        MemberSoul member = new(authorEmail, spaceId, _mockRepo.Object, _mockHelper.Object) { };
        Topic? createdTopic = null;

        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new Domain.Entities.Space());
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new Soul());
        _mockRepo.Setup(x => x.SoulRepository.IsMemberOfSpaceAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(true);
        _mockSlugHelper.Setup(x => x.CreateSlug(It.IsAny<string>()))
            .Returns("new-title");
        _mockRepo.Setup(x => x.SpaceRepository.CreateTopicAsync(It.IsAny<Topic>()))
            .Callback<Topic>(x => createdTopic = new Topic(_mockRepo.Object, _mockHelper.Object)
            {
                Id = Guid.NewGuid(),
                Title = "New Title",
                Content = "New Content",
                SpaceId = spaceId,
                SoulId = Guid.NewGuid(),
                CreatedBy = authorEmail,
                CreatedDateUtc = DateTime.UtcNow
            });

        await member.CreateTopicAsync(It.IsAny<string>(), It.IsAny<string>());

        _mockRepo.Verify(x => x.SpaceRepository.CreateTopicAsync(It.IsAny<Topic>()), Times.Once);
        Assert.NotNull(createdTopic);
        Assert.NotEqual(Guid.Empty, createdTopic?.Id);
        Assert.Equal("new-title", createdTopic?.Slug);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task UpdateTopicAsync_EmailIsInvalid_ThrowsInvalidSoulException(string email)
    {
        Guid spaceId = Guid.NewGuid();
        MemberSoul member = new(email, spaceId, _mockRepo.Object, _mockHelper.Object) { };

        await Assert.ThrowsAsync<InvalidSoulException>(() => member.UpdateTopicAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()));
    }

    [Fact]
    public async Task UpdateTopicAsync_TopicIsInvalid_ThrowsInvalidTopicIdException()
    {
        string authorEmail = "author@example.com";
        Guid spaceId = Guid.NewGuid();
        MemberSoul member = new(authorEmail, spaceId, _mockRepo.Object, _mockHelper.Object) { };

        _mockRepo.Setup(x => x.SpaceRepository.GetTopicByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Topic)null!);

        await Assert.ThrowsAsync<InvalidTopicIdException>(() => member.UpdateTopicAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()));
    }

    [Fact]
    public async Task UpdateTopicAsync_SpaceIsInvalid_ThrowsInvalidSpaceIdException()
    {
        string authorEmail = "author@example.com";
        Guid spaceId = Guid.NewGuid();
        MemberSoul member = new(authorEmail, spaceId, _mockRepo.Object, _mockHelper.Object) { };

        _mockRepo.Setup(x => x.SpaceRepository.GetTopicByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new Topic());
        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync((Domain.Entities.Space)null!);

        await Assert.ThrowsAsync<InvalidSpaceIdException>(() => member.UpdateTopicAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()));
    }

    [Fact]
    public async Task UpdateTopicAsync_TopicSpaceIdAndExistingSpaceIdDoesNotMatch_ThrowsInvalidSpaceIdException()
    {
        string authorEmail = "author@example.com";
        Guid spaceId = Guid.NewGuid();
        MemberSoul member = new(authorEmail, spaceId, _mockRepo.Object, _mockHelper.Object) { };

        _mockRepo.Setup(x => x.SpaceRepository.GetTopicByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new Topic { SpaceId = spaceId });
        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new Domain.Entities.Space { Id = Guid.NewGuid() });

        await Assert.ThrowsAsync<InvalidSpaceIdException>(() => member.UpdateTopicAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()));
    }

    [Fact]
    public async Task UpdateTopicAsync_SoulDoesNotExist_ThrowsInvalidSoulException()
    {
        string authorEmail = "author@example.com";
        Guid spaceId = Guid.NewGuid();
        MemberSoul member = new(authorEmail, spaceId, _mockRepo.Object, _mockHelper.Object) { };

        _mockRepo.Setup(x => x.SpaceRepository.GetTopicByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new Topic());
        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new Domain.Entities.Space());
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync((Soul)null!);

        await Assert.ThrowsAsync<InvalidSoulException>(() => member.UpdateTopicAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()));
    }

    [Fact]
    public async Task UpdateTopicAsync_TopicSoulIdAndExistingSoulIdDoesNotMatch_ThrowsUnauthorizedAccessException()
    {
        string authorEmail = "author@example.com";
        Guid spaceId = Guid.NewGuid();
        MemberSoul member = new(authorEmail, spaceId, _mockRepo.Object, _mockHelper.Object) { };

        _mockRepo.Setup(x => x.SpaceRepository.GetTopicByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new Topic { SoulId = Guid.NewGuid() });
        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new Domain.Entities.Space());
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new Soul { Id = Guid.NewGuid() });

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => member.UpdateTopicAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()));
    }

    [Fact]
    public async Task UpdateTopicAsync_SoulIsNotAMemberOfTheSpace_ThrowsSoulNotMemberException()
    {
        string authorEmail = "author@example.com";
        Guid topicId = Guid.NewGuid();
        Guid spaceId = Guid.NewGuid();
        Guid soulId = Guid.NewGuid();
        MemberSoul member = new(authorEmail, spaceId, _mockRepo.Object, _mockHelper.Object) { };

        _mockRepo.Setup(x => x.SpaceRepository.GetTopicByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new Topic { SoulId = soulId, SpaceId = spaceId });
        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new Domain.Entities.Space() { Id = spaceId });
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new Soul { Id = soulId });
        _mockRepo.Setup(x => x.SoulRepository.IsMemberOfSpaceAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(false);

        await Assert.ThrowsAsync<SoulNotMemberException>(() => member.UpdateTopicAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()));
    }

    [Fact]
    public async Task UpdateTopicAsync_SoulIsAModerator_UpdateTopic()
    {
        string moderatorEmail = "moderator@example.com";
        Guid topicId = Guid.NewGuid();
        Guid spaceId = Guid.NewGuid();
        Guid soulId = Guid.NewGuid();
        MemberSoul member = new(moderatorEmail, spaceId, _mockRepo.Object, _mockHelper.Object) { };
        Topic? updatedTopic = null;

        _mockRepo.Setup(x => x.SpaceRepository.GetTopicByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new Topic { SoulId = soulId, SpaceId = spaceId });
        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new Domain.Entities.Space() { Id = spaceId });
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new Soul { Id = soulId });
        _mockRepo.Setup(x => x.SoulRepository.IsModeratorOfSpaceAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(true);
        _mockRepo.Setup(x => x.SpaceRepository.UpdateTopicAsync(It.IsAny<Topic>()))
            .Callback<Topic>(x => updatedTopic = new Topic(_mockRepo.Object, _mockHelper.Object)
            {
                Title = "Updated Topic",
                Content = "Updated Content"
            });

        await member.UpdateTopicAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>());

        _mockRepo.Verify(x => x.SpaceRepository.UpdateTopicAsync(It.IsAny<Topic>()), Times.Once);
        Assert.NotNull(updatedTopic);
        Assert.Equal("Updated Topic", updatedTopic?.Title);
        Assert.Equal("Updated Content", updatedTopic?.Content);
    }

    [Fact]
    public async Task UpdateTopicAsync_SoulSpaceAndTopicAreAllValidAndSoulIsAMember_UpdateTopic()
    {
        string authorEmail = "author@example.com";
        Guid topicId = Guid.NewGuid();
        Guid spaceId = Guid.NewGuid();
        Guid soulId = Guid.NewGuid();
        MemberSoul member = new(authorEmail, spaceId, _mockRepo.Object, _mockHelper.Object) { };
        Topic? updatedTopic = null;

        _mockRepo.Setup(x => x.SpaceRepository.GetTopicByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new Topic { SoulId = soulId, SpaceId = spaceId });
        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new Domain.Entities.Space() { Id = spaceId });
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new Soul { Id = soulId });
        _mockRepo.Setup(x => x.SoulRepository.IsMemberOfSpaceAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(true);
        _mockRepo.Setup(x => x.SpaceRepository.UpdateTopicAsync(It.IsAny<Topic>()))
            .Callback<Topic>(x => updatedTopic = new Topic(_mockRepo.Object, _mockHelper.Object)
            {
                Title = "Updated Topic",
                Content = "Updated Content"
            });

        await member.UpdateTopicAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>());

        _mockRepo.Verify(x => x.SpaceRepository.UpdateTopicAsync(It.IsAny<Topic>()), Times.Once);
        Assert.NotNull(updatedTopic);
        Assert.Equal("Updated Topic", updatedTopic?.Title);
        Assert.Equal("Updated Content", updatedTopic?.Content);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task DeleteTopicAsync_EmailIsInvalid_ThrowsInvalidSoulException(string email)
    {
        Guid spaceId = Guid.NewGuid();
        MemberSoul member = new(email, spaceId, _mockRepo.Object, _mockHelper.Object) { };

        await Assert.ThrowsAsync<InvalidSoulException>(() => member.DeleteTopicAsync(It.IsAny<Guid>()));
    }

    [Fact]
    public async Task DeleteTopicAsync_TopicIsInvalid_ThrowsInvalidTopicIdException()
    {
        string authorEmail = "author@example.com";
        Guid spaceId = Guid.NewGuid();
        MemberSoul member = new(authorEmail, spaceId, _mockRepo.Object, _mockHelper.Object) { };

        _mockRepo.Setup(x => x.SpaceRepository.GetTopicByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Topic)null!);

        await Assert.ThrowsAsync<InvalidTopicIdException>(() => member.DeleteTopicAsync(It.IsAny<Guid>()));
    }

    [Fact]
    public async Task DeleteTopicAsync_SpaceIsInvalid_ThrowsInvalidSpaceIdException()
    {
        string authorEmail = "author@example.com";
        Guid spaceId = Guid.NewGuid();
        MemberSoul member = new(authorEmail, spaceId, _mockRepo.Object, _mockHelper.Object) { };

        _mockRepo.Setup(x => x.SpaceRepository.GetTopicByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new Topic());
        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync((Domain.Entities.Space)null!);

        await Assert.ThrowsAsync<InvalidSpaceIdException>(() => member.DeleteTopicAsync(It.IsAny<Guid>()));
    }

    [Fact]
    public async Task DeleteTopicAsync_TopicSpaceIdAndExistingSpaceIdDoesNotMatch_ThrowsInvalidSpaceIdException()
    {
        string authorEmail = "author@example.com";
        Guid spaceId = Guid.NewGuid();
        MemberSoul member = new(authorEmail, spaceId, _mockRepo.Object, _mockHelper.Object) { };

        _mockRepo.Setup(x => x.SpaceRepository.GetTopicByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new Topic { SpaceId = Guid.NewGuid() });
        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new Domain.Entities.Space() { Id = Guid.NewGuid() });

        await Assert.ThrowsAsync<InvalidSpaceIdException>(() => member.DeleteTopicAsync(It.IsAny<Guid>()));
    }

    [Fact]
    public async Task DeleteTopicAsync_SoulDoesNotExist_ThrowsInvalidSoulException()
    {
        string authorEmail = "author@example.com";
        Guid spaceId = Guid.NewGuid();
        MemberSoul member = new(authorEmail, spaceId, _mockRepo.Object, _mockHelper.Object) { };

        _mockRepo.Setup(x => x.SpaceRepository.GetTopicByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new Topic());
        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new Domain.Entities.Space());
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync((Soul)null!);

        await Assert.ThrowsAsync<InvalidSoulException>(() => member.DeleteTopicAsync(It.IsAny<Guid>()));
    }

    [Fact]
    public async Task DeleteTopicAsync_TopicSoulIdAndExistingSoulIdDoesNotMatch_ThrowsUnauthorizedAccessException()
    {
        string authorEmail = "author@example.com";
        Guid spaceId = Guid.NewGuid();
        MemberSoul member = new(authorEmail, spaceId, _mockRepo.Object, _mockHelper.Object) { };

        _mockRepo.Setup(x => x.SpaceRepository.GetTopicByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new Topic { SoulId = Guid.NewGuid() });
        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new Domain.Entities.Space());
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new Soul { Id = Guid.NewGuid() });

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => member.DeleteTopicAsync(It.IsAny<Guid>()));
    }

    [Fact]
    public async Task DeleteTopicAsync_SoulIsAModerator_DeleteTopic()
    {
        string authorEmail = "author@example.com";
        Guid topicId = Guid.NewGuid();
        Guid spaceId = Guid.NewGuid();
        Guid soulId = Guid.NewGuid();
        MemberSoul member = new(authorEmail, spaceId, _mockRepo.Object, _mockHelper.Object) { };
        List<Topic> topics = new()
        {
            new Topic(),
            new Topic(),
            new Topic()
        };

        _mockRepo.Setup(x => x.SpaceRepository.GetTopicByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new Topic { SoulId = soulId, SpaceId = spaceId });
        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new Domain.Entities.Space() { Id = spaceId });
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new Soul { Id = Guid.NewGuid() });
        _mockRepo.Setup(x => x.SoulRepository.IsModeratorOfSpaceAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(true);
        _mockRepo.Setup(x => x.SpaceRepository.DeleteTopicAsync(It.IsAny<Topic>()))
            .Callback<Topic>(x => topics.RemoveAt(2));

        await member.DeleteTopicAsync(It.IsAny<Guid>());

        _mockRepo.Verify(x => x.SpaceRepository.DeleteTopicAsync(It.IsAny<Topic>()), Times.Once);
        Assert.Equal(2, topics.Count);
    }

    [Fact]
    public async Task DeleteTopicAsync_SoulSpaceAndTopicAreAllValid_DeleteTopic()
    {
        string authorEmail = "author@example.com";
        Guid topicId = Guid.NewGuid();
        Guid spaceId = Guid.NewGuid();
        Guid soulId = Guid.NewGuid();
        MemberSoul member = new(authorEmail, spaceId, _mockRepo.Object, _mockHelper.Object) { };
        List<Topic> topics = new()
        {
            new Topic(),
            new Topic(),
            new Topic()
        };

        _mockRepo.Setup(x => x.SpaceRepository.GetTopicByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new Topic { SoulId = soulId, SpaceId = spaceId });
        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new Domain.Entities.Space() { Id = spaceId });
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new Soul { Id = soulId });
        _mockRepo.Setup(x => x.SpaceRepository.DeleteTopicAsync(It.IsAny<Topic>()))
            .Callback<Topic>(x => topics.RemoveAt(2));

        await member.DeleteTopicAsync(It.IsAny<Guid>());

        _mockRepo.Verify(x => x.SpaceRepository.DeleteTopicAsync(It.IsAny<Topic>()), Times.Once);
        Assert.Equal(2, topics.Count);
    }
}
