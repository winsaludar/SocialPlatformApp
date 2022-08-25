﻿using Moq;
using Space.Domain.Entities;
using Space.Domain.Exceptions;
using Space.Domain.Repositories;

namespace Space.UnitTests.Entities;

public class SoulEntityTests
{
    private readonly Mock<IRepositoryManager> _mockRepo;

    public SoulEntityTests()
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
    public async Task JoinSpaceAsync_EmailIsInvalid_ThrowsInvalidSoulException(string email)
    {
        Soul soul = new() { Email = email };
        Guid spaceId = Guid.NewGuid();

        await Assert.ThrowsAsync<InvalidSoulException>(() => soul.JoinSpaceAsync(spaceId, _mockRepo.Object));
    }

    [Fact]
    public async Task JoinSpaceAsync_SpaceIsInvalid_ThrowsInvalidSpaceIdException()
    {
        Soul soul = new() { Email = "test@example.com" };
        Guid spaceId = Guid.NewGuid();

        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Domain.Entities.Space)null!);

        await Assert.ThrowsAsync<InvalidSpaceIdException>(() => soul.JoinSpaceAsync(spaceId, _mockRepo.Object));
    }

    [Fact]
    public async Task JoinSpaceAsync_SoulDoesNotExist_CreateNewSoul()
    {
        Soul soul = new() { Email = "test@example.com" };
        Guid spaceId = Guid.NewGuid();

        Soul? createdSoul = null;
        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new Domain.Entities.Space());
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((Soul)null!);
        _mockRepo.Setup(x => x.SoulRepository.CreateAsync(It.IsAny<Soul>()))
            .Callback<Soul>(x => createdSoul = new Soul() { Id = Guid.NewGuid() });

        await soul.JoinSpaceAsync(spaceId, _mockRepo.Object);

        _mockRepo.Verify(x => x.SoulRepository.CreateAsync(soul), Times.Once);
        Assert.NotNull(createdSoul);
        Assert.NotEqual(Guid.Empty, createdSoul?.Id);
    }

    [Fact]
    public async Task JoinSpaceAsync_SoulDoesExist_SetExistingDataToCurrentSoul()
    {
        Soul currentSoul = new() { Email = "existing@example.com" };
        Soul existingSoul = new()
        {
            Id = Guid.NewGuid(),
            Email = "exisintg@example.com",
            Name = "exisintg@example.com",
            CreatedBy = "exisintg@example.com",
            CreatedDateUtc = DateTime.UtcNow
        };
        Guid spaceId = Guid.NewGuid();

        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new Domain.Entities.Space());
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(existingSoul);

        await currentSoul.JoinSpaceAsync(spaceId, _mockRepo.Object);

        Assert.Equal(existingSoul.Id, currentSoul.Id);
        Assert.Equal(existingSoul.Name, currentSoul.Name);
        Assert.Equal(existingSoul.CreatedBy, currentSoul.CreatedBy);
        Assert.Equal(existingSoul.CreatedDateUtc, currentSoul.CreatedDateUtc);
    }

    [Fact]
    public async Task JoinSpaceAsync_SoulIsAlreadyAMemberOfTheSpace_ThrowsSoulMemberAlreadyException()
    {
        Soul soul = new() { Email = "test@example.com" };
        Guid spaceId = Guid.NewGuid();

        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new Domain.Entities.Space());
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(new Soul());
        _mockRepo.Setup(x => x.SoulRepository.IsMemberOfSpace(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<SoulMemberAlreadyException>(() => soul.JoinSpaceAsync(spaceId, _mockRepo.Object));
    }

    [Fact]
    public async Task JoinSpaceAsync_SoulIsValidAndNotYetAMember_AddToSpace()
    {
        Soul soul = new() { Email = "test@example.com" };
        Soul existingSoul = new()
        {
            Id = Guid.NewGuid(),
            Email = "exisintg@example.com",
            Name = "exisintg@example.com",
            CreatedBy = "exisintg@example.com",
            CreatedDateUtc = DateTime.UtcNow
        };
        Guid spaceId = Guid.NewGuid();
        Domain.Entities.Space targetSpace = new() { Id = spaceId };

        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(targetSpace);
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(existingSoul);
        _mockRepo.Setup(x => x.SoulRepository.IsMemberOfSpace(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(false);

        await soul.JoinSpaceAsync(spaceId, _mockRepo.Object);

        _mockRepo.Verify(x => x.SpaceRepository.UpdateAsync(targetSpace), Times.Once);
        _mockRepo.Verify(x => x.UnitOfWork.CommitAsync(), Times.Once);
        Assert.Equal(1, targetSpace.Souls.Count);
    }
}
