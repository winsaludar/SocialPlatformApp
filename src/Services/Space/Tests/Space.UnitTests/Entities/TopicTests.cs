using Moq;
using Space.Domain.Entities;
using Space.Domain.Exceptions;
using Space.Domain.Helpers;
using Space.Domain.Repositories;

namespace Space.UnitTests.Entities;

public class TopicTests
{
    private readonly Mock<IRepositoryManager> _mockRepo;
    private readonly Mock<IHelperManager> _mockHelper;

    public TopicTests()
    {
        Mock<IUnitOfWork> mockUnitOfWork = new();
        Mock<ISpaceRepository> mockSpaceRepo = new();
        Mock<ISoulRepository> mockSoulRepo = new();
        _mockRepo = new Mock<IRepositoryManager>();
        _mockRepo.Setup(x => x.UnitOfWork).Returns(mockUnitOfWork.Object);
        _mockRepo.Setup(x => x.SpaceRepository).Returns(mockSpaceRepo.Object);
        _mockRepo.Setup(x => x.SoulRepository).Returns(mockSoulRepo.Object);

        Mock<ISlugHelper> mockSlugHelper = new();
        _mockHelper = new Mock<IHelperManager>();
        _mockHelper.Setup(x => x.SlugHelper).Returns(mockSlugHelper.Object);
    }

    [Fact]
    public async Task UpvoteAsync_RepositoryManagerIsNull_ThrowsNullReferenceException()
    {
        Topic topic = new();

        await Assert.ThrowsAsync<NullReferenceException>(() => topic.UpvoteAsync(It.IsAny<string>()));
    }

    [Fact]
    public async Task UpvoteAsync_TopicDoesNotExist_ThrowsInvalidTopicIdException()
    {
        Topic topic = new(_mockRepo.Object, _mockHelper.Object);

        _mockRepo.Setup(x => x.SpaceRepository.GetTopicByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
            .ReturnsAsync((Topic)null!);

        await Assert.ThrowsAsync<InvalidTopicIdException>(() => topic.UpvoteAsync(It.IsAny<string>()));
    }

    [Fact]
    public async Task UpvoteAsync_TopicSpaceIdIsDifferent_ThrowsInvalidTopicIdException()
    {
        Topic topic = new(_mockRepo.Object, _mockHelper.Object) { Id = Guid.NewGuid(), SpaceId = Guid.NewGuid() };

        _mockRepo.Setup(x => x.SpaceRepository.GetTopicByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
            .ReturnsAsync(new Topic() { SpaceId = Guid.NewGuid() });

        await Assert.ThrowsAsync<InvalidTopicIdException>(() => topic.UpvoteAsync(It.IsAny<string>()));
    }

    [Fact]
    public async Task UpvoteAsync_VoterDoesNotExist_ThrowsInvalidSoulException()
    {
        Topic topic = new(_mockRepo.Object, _mockHelper.Object) { Id = Guid.NewGuid(), SpaceId = Guid.NewGuid() };

        _mockRepo.Setup(x => x.SpaceRepository.GetTopicByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
            .ReturnsAsync(topic);
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync((Soul)null!);

        await Assert.ThrowsAsync<InvalidSoulException>(() => topic.UpvoteAsync(It.IsAny<string>()));
    }

    [Fact]
    public async Task UpvoteAsync_TopicSpaceAndSoulAreAllValidAndNoVoteYet_CreateVote()
    {
        Topic topic = new(_mockRepo.Object, _mockHelper.Object) { Id = Guid.NewGuid(), SpaceId = Guid.NewGuid() };
        SoulTopicVote? newVote = null;

        _mockRepo.Setup(x => x.SpaceRepository.GetTopicByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
            .ReturnsAsync(topic);
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new Soul());
        _mockRepo.Setup(x => x.SoulRepository.GetTopicVoteAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync((SoulTopicVote)null!);
        _mockRepo.Setup(x => x.SoulRepository.CreateTopicVoteAsync(It.IsAny<SoulTopicVote>()))
            .Callback<SoulTopicVote>(x => newVote = new SoulTopicVote { Upvote = 1, Downvote = 0 });

        await topic.UpvoteAsync(It.IsAny<string>());

        _mockRepo.Verify(x => x.SoulRepository.CreateTopicVoteAsync(It.IsAny<SoulTopicVote>()), Times.Once);
        _mockRepo.Verify(x => x.UnitOfWork.CommitAsync(), Times.Once);
        Assert.NotNull(newVote);
        Assert.Equal(1, newVote?.Upvote);
        Assert.Equal(0, newVote?.Downvote);
    }

    [Fact]
    public async Task UpvoteAsync_TopicSpaceAndSoulAreAllValidAndHasVoteAlready_UpdateVote()
    {
        Topic topic = new(_mockRepo.Object, _mockHelper.Object) { Id = Guid.NewGuid(), SpaceId = Guid.NewGuid() };
        SoulTopicVote vote = new() { Upvote = 0, Downvote = 1 };

        _mockRepo.Setup(x => x.SpaceRepository.GetTopicByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
            .ReturnsAsync(topic);
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new Soul());
        _mockRepo.Setup(x => x.SoulRepository.GetTopicVoteAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(vote);
        _mockRepo.Setup(x => x.SoulRepository.UpdateTopicVoteAsync(It.IsAny<SoulTopicVote>()))
            .Callback<SoulTopicVote>(x => vote = new SoulTopicVote { Upvote = 1, Downvote = 0 });

        await topic.UpvoteAsync(It.IsAny<string>());

        _mockRepo.Verify(x => x.SoulRepository.UpdateTopicVoteAsync(It.IsAny<SoulTopicVote>()), Times.Once);
        _mockRepo.Verify(x => x.UnitOfWork.CommitAsync(), Times.Once);
        Assert.Equal(1, vote.Upvote);
        Assert.Equal(0, vote.Downvote);
    }

    [Fact]
    public async Task DownvoteAsync_RepositoryManagerIsNull_ThrowsNullReferenceException()
    {
        Topic topic = new();

        await Assert.ThrowsAsync<NullReferenceException>(() => topic.DownvoteAsync(It.IsAny<string>()));
    }

    [Fact]
    public async Task DownvoteAsync_TopicDoesNotExist_ThrowsInvalidTopicIdException()
    {
        Topic topic = new(_mockRepo.Object, _mockHelper.Object);

        _mockRepo.Setup(x => x.SpaceRepository.GetTopicByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
            .ReturnsAsync((Topic)null!);

        await Assert.ThrowsAsync<InvalidTopicIdException>(() => topic.DownvoteAsync(It.IsAny<string>()));
    }

    [Fact]
    public async Task DownvoteAsync_TopicSpaceIdIsDifferent_ThrowsInvalidTopicIdException()
    {
        Topic topic = new(_mockRepo.Object, _mockHelper.Object) { Id = Guid.NewGuid(), SpaceId = Guid.NewGuid() };

        _mockRepo.Setup(x => x.SpaceRepository.GetTopicByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
            .ReturnsAsync(new Topic() { SpaceId = Guid.NewGuid() });

        await Assert.ThrowsAsync<InvalidTopicIdException>(() => topic.DownvoteAsync(It.IsAny<string>()));
    }

    [Fact]
    public async Task DownvoteAsync_VoterDoesNotExist_ThrowsInvalidSoulException()
    {
        Topic topic = new(_mockRepo.Object, _mockHelper.Object) { Id = Guid.NewGuid(), SpaceId = Guid.NewGuid() };

        _mockRepo.Setup(x => x.SpaceRepository.GetTopicByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
            .ReturnsAsync(topic);
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync((Soul)null!);

        await Assert.ThrowsAsync<InvalidSoulException>(() => topic.DownvoteAsync(It.IsAny<string>()));
    }

    [Fact]
    public async Task DownvoteAsync_TopicSpaceAndSoulAreAllValidAndNoVoteYet_CreateVote()
    {
        Topic topic = new(_mockRepo.Object, _mockHelper.Object) { Id = Guid.NewGuid(), SpaceId = Guid.NewGuid() };
        SoulTopicVote? newVote = null;

        _mockRepo.Setup(x => x.SpaceRepository.GetTopicByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
            .ReturnsAsync(topic);
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new Soul());
        _mockRepo.Setup(x => x.SoulRepository.GetTopicVoteAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync((SoulTopicVote)null!);
        _mockRepo.Setup(x => x.SoulRepository.CreateTopicVoteAsync(It.IsAny<SoulTopicVote>()))
            .Callback<SoulTopicVote>(x => newVote = new SoulTopicVote { Upvote = 0, Downvote = 1 });

        await topic.DownvoteAsync(It.IsAny<string>());

        _mockRepo.Verify(x => x.SoulRepository.CreateTopicVoteAsync(It.IsAny<SoulTopicVote>()), Times.Once);
        _mockRepo.Verify(x => x.UnitOfWork.CommitAsync(), Times.Once);
        Assert.NotNull(newVote);
        Assert.Equal(0, newVote?.Upvote);
        Assert.Equal(1, newVote?.Downvote);
    }

    [Fact]
    public async Task DownvoteAsync_TopicSpaceAndSoulAreAllValidAndHasVoteAlready_UpdateVote()
    {
        Topic topic = new(_mockRepo.Object, _mockHelper.Object) { Id = Guid.NewGuid(), SpaceId = Guid.NewGuid() };
        SoulTopicVote vote = new() { Upvote = 1, Downvote = 0 };

        _mockRepo.Setup(x => x.SpaceRepository.GetTopicByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
            .ReturnsAsync(topic);
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new Soul());
        _mockRepo.Setup(x => x.SoulRepository.GetTopicVoteAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(vote);
        _mockRepo.Setup(x => x.SoulRepository.UpdateTopicVoteAsync(It.IsAny<SoulTopicVote>()))
            .Callback<SoulTopicVote>(x => vote = new SoulTopicVote { Upvote = 0, Downvote = 1 });

        await topic.DownvoteAsync(It.IsAny<string>());

        _mockRepo.Verify(x => x.SoulRepository.UpdateTopicVoteAsync(It.IsAny<SoulTopicVote>()), Times.Once);
        _mockRepo.Verify(x => x.UnitOfWork.CommitAsync(), Times.Once);
        Assert.Equal(0, vote.Upvote);
        Assert.Equal(1, vote.Downvote);
    }

    [Fact]
    public async Task UnvoteAsync_RepositoryManagerIsNull_ThrowsNullReferenceException()
    {
        Topic topic = new();

        await Assert.ThrowsAsync<NullReferenceException>(() => topic.UnvoteAsync(It.IsAny<string>()));
    }

    [Fact]
    public async Task UnvoteAsync_TopicDoesNotExist_ThrowsInvalidTopicIdException()
    {
        Topic topic = new(_mockRepo.Object, _mockHelper.Object);

        _mockRepo.Setup(x => x.SpaceRepository.GetTopicByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
            .ReturnsAsync((Topic)null!);

        await Assert.ThrowsAsync<InvalidTopicIdException>(() => topic.UnvoteAsync(It.IsAny<string>()));
    }

    [Fact]
    public async Task UnvoteAsync_TopicSpaceIdIsDifferent_ThrowsInvalidTopicIdException()
    {
        Topic topic = new(_mockRepo.Object, _mockHelper.Object) { Id = Guid.NewGuid(), SpaceId = Guid.NewGuid() };

        _mockRepo.Setup(x => x.SpaceRepository.GetTopicByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
            .ReturnsAsync(new Topic() { SpaceId = Guid.NewGuid() });

        await Assert.ThrowsAsync<InvalidTopicIdException>(() => topic.UnvoteAsync(It.IsAny<string>()));
    }

    [Fact]
    public async Task UnvoteAsync_VoterDoesNotExist_ThrowsInvalidSoulException()
    {
        Topic topic = new(_mockRepo.Object, _mockHelper.Object) { Id = Guid.NewGuid(), SpaceId = Guid.NewGuid() };

        _mockRepo.Setup(x => x.SpaceRepository.GetTopicByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
            .ReturnsAsync(topic);
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync((Soul)null!);

        await Assert.ThrowsAsync<InvalidSoulException>(() => topic.UnvoteAsync(It.IsAny<string>()));
    }

    [Fact]
    public async Task UnvoteAsync_TopicSpaceAndSoulAreAllValidAndNoVoteYet_CreateVote()
    {
        Topic topic = new(_mockRepo.Object, _mockHelper.Object) { Id = Guid.NewGuid(), SpaceId = Guid.NewGuid() };
        SoulTopicVote? newVote = null;

        _mockRepo.Setup(x => x.SpaceRepository.GetTopicByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
            .ReturnsAsync(topic);
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new Soul());
        _mockRepo.Setup(x => x.SoulRepository.GetTopicVoteAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync((SoulTopicVote)null!);
        _mockRepo.Setup(x => x.SoulRepository.CreateTopicVoteAsync(It.IsAny<SoulTopicVote>()))
            .Callback<SoulTopicVote>(x => newVote = new SoulTopicVote { Upvote = 0, Downvote = 0 });

        await topic.UnvoteAsync(It.IsAny<string>());

        _mockRepo.Verify(x => x.SoulRepository.CreateTopicVoteAsync(It.IsAny<SoulTopicVote>()), Times.Once);
        _mockRepo.Verify(x => x.UnitOfWork.CommitAsync(), Times.Once);
        Assert.NotNull(newVote);
        Assert.Equal(0, newVote?.Upvote);
        Assert.Equal(0, newVote?.Downvote);
    }

    [Fact]
    public async Task UnvoteAsync_TopicSpaceAndSoulAreAllValidAndHasVoteAlready_UpdateVote()
    {
        Topic topic = new(_mockRepo.Object, _mockHelper.Object) { Id = Guid.NewGuid(), SpaceId = Guid.NewGuid() };
        SoulTopicVote vote = new() { Upvote = 1, Downvote = 0 };

        _mockRepo.Setup(x => x.SpaceRepository.GetTopicByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
            .ReturnsAsync(topic);
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new Soul());
        _mockRepo.Setup(x => x.SoulRepository.GetTopicVoteAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(vote);
        _mockRepo.Setup(x => x.SoulRepository.UpdateTopicVoteAsync(It.IsAny<SoulTopicVote>()))
            .Callback<SoulTopicVote>(x => vote = new SoulTopicVote { Upvote = 0, Downvote = 0 });

        await topic.UnvoteAsync(It.IsAny<string>());

        _mockRepo.Verify(x => x.SoulRepository.UpdateTopicVoteAsync(It.IsAny<SoulTopicVote>()), Times.Once);
        _mockRepo.Verify(x => x.UnitOfWork.CommitAsync(), Times.Once);
        Assert.Equal(0, vote.Upvote);
        Assert.Equal(0, vote.Downvote);
    }

    [Fact]
    public async Task AddCommentAsync_RepositoryManagerIsNull_ThrowsNullReferenceException()
    {
        Topic topic = new();

        await Assert.ThrowsAsync<NullReferenceException>(() => topic.AddCommentAsync(It.IsAny<string>(), It.IsAny<Comment>()));
    }

    [Fact]
    public async Task AddCommentAsync_TopicDoesNotExist_ThrowsInvalidTopicIdException()
    {
        Topic topic = new(_mockRepo.Object, _mockHelper.Object);

        _mockRepo.Setup(x => x.SpaceRepository.GetTopicByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
            .ReturnsAsync((Topic)null!);

        await Assert.ThrowsAsync<InvalidTopicIdException>(() => topic.AddCommentAsync(It.IsAny<string>(), It.IsAny<Comment>()));
    }

    [Fact]
    public async Task AddCommentAsync_TopicSpaceIdIsDifferent_ThrowsInvalidTopicIdException()
    {
        Topic topic = new(_mockRepo.Object, _mockHelper.Object) { Id = Guid.NewGuid(), SpaceId = Guid.NewGuid() };

        _mockRepo.Setup(x => x.SpaceRepository.GetTopicByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
            .ReturnsAsync(new Topic() { SpaceId = Guid.NewGuid() });

        await Assert.ThrowsAsync<InvalidTopicIdException>(() => topic.AddCommentAsync(It.IsAny<string>(), It.IsAny<Comment>()));
    }

    [Fact]
    public async Task AddCommentAsync_AuthorDoesNotExist_ThrowsInvalidSoulException()
    {
        Topic topic = new(_mockRepo.Object, _mockHelper.Object) { Id = Guid.NewGuid(), SpaceId = Guid.NewGuid() };

        _mockRepo.Setup(x => x.SpaceRepository.GetTopicByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
            .ReturnsAsync(topic);
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync((Soul)null!);

        await Assert.ThrowsAsync<InvalidSoulException>(() => topic.AddCommentAsync(It.IsAny<string>(), It.IsAny<Comment>()));
    }

    [Fact]
    public async Task AddCommentAsync_TopicSpaceAndSoulAreAllValid_CreateComment()
    {
        Topic topic = new(_mockRepo.Object, _mockHelper.Object) { Id = Guid.NewGuid(), SpaceId = Guid.NewGuid() };
        Soul author = new() { Id = Guid.NewGuid() };
        Comment newComment = new() { TopicId = topic.Id, SoulId = author.Id };
        Comment? createdComment = null;

        _mockRepo.Setup(x => x.SpaceRepository.GetTopicByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
            .ReturnsAsync(topic);
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(author);
        _mockRepo.Setup(x => x.SpaceRepository.CreateCommentAsync(It.IsAny<Comment>()))
            .Callback<Comment>(x => createdComment = newComment);

        await topic.AddCommentAsync(It.IsAny<string>(), newComment);

        _mockRepo.Verify(x => x.SpaceRepository.CreateCommentAsync(It.IsAny<Comment>()), Times.Once);
        _mockRepo.Verify(x => x.UnitOfWork.CommitAsync(), Times.Once);
        Assert.NotNull(createdComment);
        Assert.Equal(topic.Id, createdComment?.TopicId);
        Assert.Equal(author.Id, createdComment?.SoulId);
    }
}
