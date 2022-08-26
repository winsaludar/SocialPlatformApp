using Moq;
using Space.Domain.Entities;
using Space.Domain.Exceptions;
using Space.Domain.Repositories;
using DomainEntities = Space.Domain.Entities;

namespace Space.UnitTests.Entities;

public class SpaceTests
{
    private readonly Mock<IRepositoryManager> _mockRepo;

    public SpaceTests()
    {
        Mock<IUnitOfWork> mockUnitOfWork = new();
        Mock<ISpaceRepository> mockSpaceRepo = new();
        Mock<ISoulRepository> mockSoulRepo = new();
        _mockRepo = new Mock<IRepositoryManager>();
        _mockRepo.Setup(x => x.UnitOfWork).Returns(mockUnitOfWork.Object);
        _mockRepo.Setup(x => x.SpaceRepository).Returns(mockSpaceRepo.Object);
        _mockRepo.Setup(x => x.SoulRepository).Returns(mockSoulRepo.Object);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task KickSoulAsync_EmailIsInvalid_ThrowsInvalidSoulException(string email)
    {
        DomainEntities.Space space = new() { };

        await Assert.ThrowsAsync<InvalidSoulException>(() => space.KickSoulAsync(email, _mockRepo.Object));
    }

    [Fact]
    public async Task KickSoulAsync_SpaceIsInvalid_ThrowsInvalidSpaceIdException()
    {
        DomainEntities.Space space = new() { Id = Guid.NewGuid() };
        string email = "member@example.com";

        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
            .ReturnsAsync((DomainEntities.Space)null!);

        await Assert.ThrowsAsync<InvalidSpaceIdException>(() => space.KickSoulAsync(email, _mockRepo.Object));
    }

    [Fact]
    public async Task KickSoulAsync_SoulDoesNotExist_ThrowsInvalidSoulException()
    {
        DomainEntities.Space space = new() { Id = Guid.NewGuid() };
        string email = "notexisting@example.com";

        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
            .ReturnsAsync(new DomainEntities.Space());
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync((Soul)null!);

        await Assert.ThrowsAsync<InvalidSoulException>(() => space.KickSoulAsync(email, _mockRepo.Object));
    }

    [Fact]
    public async Task KickSoulAsync_SoulIsNotAMemberOfTheSpace_ThrowsSoulNotMemberException()
    {
        DomainEntities.Space space = new() { Id = Guid.NewGuid() };
        string email = "notmember@example.com";

        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>()))
            .ReturnsAsync(new DomainEntities.Space());
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(new Soul());
        _mockRepo.Setup(x => x.SoulRepository.IsMemberOfSpaceAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(false);

        await Assert.ThrowsAsync<SoulNotMemberException>(() => space.KickSoulAsync(email, _mockRepo.Object));
    }

    [Fact]
    public async Task KickSoulAsync_SoulAndSpaceAreBothValidAndSoulIsAMember_RemoveToSpace()
    {
        DomainEntities.Space space = new() { Id = Guid.NewGuid() };
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

        await space.KickSoulAsync(existingSoul.Email, _mockRepo.Object);

        _mockRepo.Verify(x => x.SoulRepository.DeleteSoulSpaceAsync(existingSoul.Id, space.Id), Times.Once);
        _mockRepo.Verify(x => x.UnitOfWork.CommitAsync(), Times.Once);
    }
}
