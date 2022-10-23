using Moq;
using Space.Domain.Entities;
using Space.Domain.Exceptions;
using Space.Domain.Helpers;
using Space.Domain.Repositories;

namespace Space.UnitTests.Entities;

public class ModeratorSoulTests
{
    private readonly Mock<IRepositoryManager> _mockRepo;
    private readonly Mock<IHelperManager> _mockHelper;

    public ModeratorSoulTests()
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

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task KickMemberAsync_ModeratorEmailIsInvalid_ThrowsInvalidSoulException(string moderatorEmail)
    {
        string memberEmail = "member@example.com";
        Guid spaceId = Guid.NewGuid();
        ModeratorSoul moderator = new(moderatorEmail, spaceId, _mockRepo.Object, _mockHelper.Object) { };

        await Assert.ThrowsAsync<InvalidSoulException>(() => moderator.KickMemberAsync(memberEmail));
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task KickMemberAsync_MemberEmailIsInvalid_ThrowsInvalidSoulException(string memberEmail)
    {
        string moderatorEmail = "member@example.com";
        Guid spaceId = Guid.NewGuid();
        ModeratorSoul moderator = new(moderatorEmail, spaceId, _mockRepo.Object, _mockHelper.Object) { };

        await Assert.ThrowsAsync<InvalidSoulException>(() => moderator.KickMemberAsync(memberEmail));
    }

    [Fact]
    public async Task KickMemberAsync_SpaceIsInvalid_ThrowsInvalidSpaceIdException()
    {
        string moderatorEmail = "moderator@example.com";
        string memberEmail = "member@example.com";
        Guid spaceId = Guid.NewGuid();
        ModeratorSoul moderator = new(moderatorEmail, spaceId, _mockRepo.Object, _mockHelper.Object) { };

        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync((Domain.Entities.Space)null!);

        await Assert.ThrowsAsync<InvalidSpaceIdException>(() => moderator.KickMemberAsync(memberEmail));
    }

    [Fact]
    public async Task KickMemberAsync_ModeratorSoulOrMemberSoulDoesNotExist_ThrowsInvalidSoulException()
    {
        string moderatorEmail = "moderator@example.com";
        string memberEmail = "member@example.com";
        Guid spaceId = Guid.NewGuid();
        ModeratorSoul moderator = new(moderatorEmail, spaceId, _mockRepo.Object, _mockHelper.Object) { };

        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new Domain.Entities.Space());
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync((Soul)null!);

        await Assert.ThrowsAsync<InvalidSoulException>(() => moderator.KickMemberAsync(memberEmail));
    }

    public async Task KickMemberAsync_ModeratorSoulIsNotAModeratorOfTheSpace_ThrowsUnauthorizedAccessException()
    {
        string moderatorEmail = "moderator@example.com";
        string memberEmail = "member@example.com";
        Guid spaceId = Guid.NewGuid();
        ModeratorSoul moderator = new(moderatorEmail, spaceId, _mockRepo.Object, _mockHelper.Object) { };

        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new Domain.Entities.Space());
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new Soul());
        _mockRepo.Setup(x => x.SoulRepository.IsModeratorOfSpaceAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(false);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => moderator.KickMemberAsync(memberEmail));
    }

    [Fact]
    public async Task KickMemberAsync_MemberSoulIsNotAMemberOfTheSpace_ThrowsSoulNotMemberException()
    {
        string moderatorEmail = "moderator@example.com";
        string memberEmail = "member@example.com";
        Guid spaceId = Guid.NewGuid();
        ModeratorSoul moderator = new(moderatorEmail, spaceId, _mockRepo.Object, _mockHelper.Object) { };

        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new Domain.Entities.Space());
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new Soul());
        _mockRepo.Setup(x => x.SoulRepository.IsModeratorOfSpaceAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(true);
        _mockRepo.Setup(x => x.SoulRepository.IsMemberOfSpaceAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(false);

        await Assert.ThrowsAsync<SoulNotMemberException>(() => moderator.KickMemberAsync(memberEmail));
    }

    [Fact]
    public async Task KickMemberAsync_SoulAndSpaceAreBothValidAndKickedByIsAModeratorAndMemberSoulIsAMember_RemoveToSpace()
    {
        string moderatorEmail = "moderator@example.com";
        string memberEmail = "member@example.com";
        Guid spaceId = Guid.NewGuid();
        ModeratorSoul moderator = new(moderatorEmail, spaceId, _mockRepo.Object, _mockHelper.Object) { };

        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new Domain.Entities.Space());
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new Soul());
        _mockRepo.Setup(x => x.SoulRepository.IsModeratorOfSpaceAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(true);
        _mockRepo.Setup(x => x.SoulRepository.IsMemberOfSpaceAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(true);

        await moderator.KickMemberAsync(memberEmail);

        _mockRepo.Verify(x => x.SoulRepository.DeleteSpaceMemberAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Once);
        _mockRepo.Verify(x => x.UnitOfWork.CommitAsync(), Times.Once);
    }
}
