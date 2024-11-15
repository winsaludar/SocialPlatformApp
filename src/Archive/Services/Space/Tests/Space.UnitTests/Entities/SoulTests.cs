﻿using Moq;
using Space.Domain.Entities;
using Space.Domain.Exceptions;
using Space.Domain.Repositories;

namespace Space.UnitTests.Entities;

public class SoulTests
{
    private readonly Mock<IRepositoryManager> _mockRepo;

    public SoulTests()
    {
        Mock<IUnitOfWork> mockUnitOfWork = new();
        Mock<ISpaceRepository> mockSpaceRepo = new();
        Mock<ISoulRepository> mockSoulRepo = new();
        _mockRepo = new Mock<IRepositoryManager>();
        _mockRepo.Setup(x => x.UnitOfWork).Returns(mockUnitOfWork.Object);
        _mockRepo.Setup(x => x.SpaceRepository).Returns(mockSpaceRepo.Object);
        _mockRepo.Setup(x => x.SoulRepository).Returns(mockSoulRepo.Object);
    }

    [Fact]
    public async Task CreateSpaceAsync_RepositoryManagerIsNull_ThrowsNullReferenceException()
    {
        Soul soul = new() { Name = "test@example.com" };

        await Assert.ThrowsAsync<NullReferenceException>(() => soul.CreateSpaceAsync(It.IsAny<Domain.Entities.Space>()));
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task CreateSpaceAsync_SpaceNameIsInvalid_ThrowsInvalidSpaceNameException(string name)
    {
        Soul soul = new(_mockRepo.Object) { Name = "test@example.com" };
        Domain.Entities.Space newSpace = new() { Name = name };

        await Assert.ThrowsAsync<InvalidSpaceNameException>(() => soul.CreateSpaceAsync(newSpace));
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task CreateSpaceAsync_SpaceCreatorIsInvalid_ThrowsInvalidSoulException(string creator)
    {
        Soul soul = new(_mockRepo.Object) { Name = creator };
        Domain.Entities.Space newSpace = new() { Name = "Test Space" };

        await Assert.ThrowsAsync<InvalidSoulException>(() => soul.CreateSpaceAsync(newSpace));
    }

    [Fact]
    public async Task CreateSpaceAsync_SoulDoesNotExist_CreateNewSoul()
    {
        Soul soul = new(_mockRepo.Object) { Email = "test@example.com" };
        Domain.Entities.Space newSpace = new() { Name = "Test Space" };

        Soul? createdSoul = null;
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .ReturnsAsync((Soul)null!);
        _mockRepo.Setup(x => x.SoulRepository.CreateAsync(It.IsAny<Soul>()))
                .Callback<Soul>(x => createdSoul = new Soul() { Id = Guid.NewGuid(), Email = soul.Email });

        await soul.CreateSpaceAsync(newSpace);

        _mockRepo.Verify(x => x.SoulRepository.CreateAsync(It.IsAny<Soul>()), Times.Once);
        Assert.NotNull(createdSoul);
        Assert.NotEqual(Guid.Empty, createdSoul?.Id);
    }

    [Fact]
    public async Task CreateSpaceAsync_SpaceNameAlreadyExist_ThrowsSpaceNameAlreadyExistException()
    {
        Soul soul = new(_mockRepo.Object) { Email = "test@example.com" };
        Domain.Entities.Space newSpace = new() { Name = "Test Space" };

        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new Soul());
        _mockRepo.Setup(x => x.SpaceRepository.GetByNameAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new Domain.Entities.Space());

        await Assert.ThrowsAsync<SpaceNameAlreadyExistException>(() => soul.CreateSpaceAsync(newSpace));
    }

    [Fact]
    public async Task CreateSpaceAsync_SpaceIsValid_CreateSpace()
    {
        Domain.Entities.Space? createdSpace = null;
        Soul soul = new(_mockRepo.Object) { Email = "test@example.com" };
        Domain.Entities.Space newSpace = new() { Name = "Test Space" };

        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new Soul());
        _mockRepo.Setup(x => x.SpaceRepository.GetByNameAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync((Domain.Entities.Space)null!);
        _mockRepo.Setup(x => x.SpaceRepository.CreateAsync(It.IsAny<Domain.Entities.Space>()))
            .Callback<Domain.Entities.Space>(x => createdSpace = x);

        await soul.CreateSpaceAsync(newSpace);

        _mockRepo.Verify(x => x.SpaceRepository.CreateAsync(It.IsAny<Domain.Entities.Space>()), Times.Once);
        Assert.Equal(newSpace.Name, createdSpace?.Name);
        Assert.Equal(newSpace.Creator, createdSpace?.Creator);
        Assert.Equal(newSpace.ShortDescription, createdSpace?.ShortDescription);
        Assert.Equal(newSpace.LongDescription, createdSpace?.LongDescription);
        Assert.Equal(1, createdSpace?.Members.Count);
    }

    [Fact]
    public async Task JoinSpaceAsync_RepositoryManagerIsNull_ThrowsNullReferenceException()
    {
        Soul soul = new() { Name = "test@example.com" };

        await Assert.ThrowsAsync<NullReferenceException>(() => soul.JoinSpaceAsync(It.IsAny<Guid>()));
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task JoinSpaceAsync_EmailIsInvalid_ThrowsInvalidSoulException(string email)
    {
        Soul soul = new(_mockRepo.Object) { Email = email };
        Guid spaceId = Guid.NewGuid();

        await Assert.ThrowsAsync<InvalidSoulException>(() => soul.JoinSpaceAsync(spaceId));
    }

    [Fact]
    public async Task JoinSpaceAsync_SpaceIsInvalid_ThrowsInvalidSpaceIdException()
    {
        Soul soul = new(_mockRepo.Object) { Email = "test@example.com" };
        Guid spaceId = Guid.NewGuid();

        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync((Domain.Entities.Space)null!);

        await Assert.ThrowsAsync<InvalidSpaceIdException>(() => soul.JoinSpaceAsync(spaceId));
    }

    [Fact]
    public async Task JoinSpaceAsync_SoulDoesNotExist_CreateNewSoul()
    {
        Soul soul = new(_mockRepo.Object) { Email = "test@example.com" };
        Guid spaceId = Guid.NewGuid();

        Soul? createdSoul = null;
        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new Domain.Entities.Space());
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync((Soul)null!);
        _mockRepo.Setup(x => x.SoulRepository.CreateAsync(It.IsAny<Soul>()))
            .Callback<Soul>(x => createdSoul = new Soul() { Id = Guid.NewGuid() });

        await soul.JoinSpaceAsync(spaceId);

        _mockRepo.Verify(x => x.SoulRepository.CreateAsync(It.IsAny<Soul>()), Times.Once);
        Assert.NotNull(createdSoul);
        Assert.NotEqual(Guid.Empty, createdSoul?.Id);
    }

    [Fact]
    public async Task JoinSpaceAsync_SoulIsAlreadyAMemberOfTheSpace_ThrowsSoulMemberAlreadyException()
    {
        Soul soul = new(_mockRepo.Object) { Email = "test@example.com" };
        Guid spaceId = Guid.NewGuid();

        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new Domain.Entities.Space());
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new Soul());
        _mockRepo.Setup(x => x.SoulRepository.IsMemberOfSpaceAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<SoulMemberAlreadyException>(() => soul.JoinSpaceAsync(spaceId));
    }

    [Fact]
    public async Task JoinSpaceAsync_SoulIsValidAndNotYetAMember_AddToSpace()
    {
        Soul soul = new(_mockRepo.Object) { Email = "test@example.com" };
        Soul existingSoul = new()
        {
            Id = Guid.NewGuid(),
            Email = "existing@example.com",
            Name = "existing@example.com",
            CreatedBy = "existing@example.com",
            CreatedDateUtc = DateTime.UtcNow
        };
        Guid spaceId = Guid.NewGuid();
        Domain.Entities.Space targetSpace = new() { Id = spaceId };

        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(targetSpace);
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(existingSoul);
        _mockRepo.Setup(x => x.SoulRepository.IsMemberOfSpaceAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(false);

        await soul.JoinSpaceAsync(spaceId);

        _mockRepo.Verify(x => x.SpaceRepository.UpdateAsync(targetSpace), Times.Once);
        _mockRepo.Verify(x => x.UnitOfWork.CommitAsync(), Times.Once);
        Assert.Equal(1, targetSpace.Members.Count);
    }

    [Fact]
    public async Task LeaveSpaceAsync_RepositoryManagerIsNull_ThrowsNullReferenceException()
    {
        Soul soul = new() { Name = "test@example.com" };

        await Assert.ThrowsAsync<NullReferenceException>(() => soul.LeaveSpaceAsync(It.IsAny<Guid>()));
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task LeaveSpaceAsync_EmailIsInvalid_ThrowsInvalidSoulException(string email)
    {
        Soul soul = new(_mockRepo.Object) { Email = email };
        Guid spaceId = Guid.NewGuid();

        await Assert.ThrowsAsync<InvalidSoulException>(() => soul.LeaveSpaceAsync(spaceId));
    }

    [Fact]
    public async Task LeaveSpaceAsync_SpaceIsInvalid_ThrowsInvalidSpaceIdException()
    {
        Soul soul = new(_mockRepo.Object) { Email = "test@example.com" };
        Guid spaceId = Guid.NewGuid();

        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync((Domain.Entities.Space)null!);

        await Assert.ThrowsAsync<InvalidSpaceIdException>(() => soul.LeaveSpaceAsync(spaceId));
    }

    [Fact]
    public async Task LeaveSpaceAsync_SoulDoesNotExist_ThrowsInvalidSoulException()
    {
        Soul soul = new(_mockRepo.Object) { Email = "test@example.com" };
        Guid spaceId = Guid.NewGuid();

        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new Domain.Entities.Space());
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync((Soul)null!);

        await Assert.ThrowsAsync<InvalidSoulException>(() => soul.LeaveSpaceAsync(spaceId));
    }

    [Fact]
    public async Task LeaveSpaceAsync_SoulIsNotAMemberOfTheSpace_ThrowsSoulNotMemberException()
    {
        Soul soul = new(_mockRepo.Object) { Email = "test@example.com" };
        Guid spaceId = Guid.NewGuid();

        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new Domain.Entities.Space());
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(new Soul());
        _mockRepo.Setup(x => x.SoulRepository.IsMemberOfSpaceAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(false);

        await Assert.ThrowsAsync<SoulNotMemberException>(() => soul.LeaveSpaceAsync(spaceId));
    }

    [Fact]
    public async Task LeaveSpaceAsync_SoulAndSpaceAreBothValidAndSoulIsAMember_RemoveToSpace()
    {
        Soul soul = new(_mockRepo.Object) { Email = "test@example.com" };
        Soul existingSoul = new()
        {
            Id = Guid.NewGuid(),
            Email = "existing@example.com",
            Name = "existing@example.com",
            CreatedBy = "existing@example.com",
            CreatedDateUtc = DateTime.UtcNow
        };
        Guid spaceId = Guid.NewGuid();
        Domain.Entities.Space targetSpace = new() { Id = spaceId };

        _mockRepo.Setup(x => x.SpaceRepository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(targetSpace);
        _mockRepo.Setup(x => x.SoulRepository.GetByEmailAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(existingSoul);
        _mockRepo.Setup(x => x.SoulRepository.IsMemberOfSpaceAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(true);

        await soul.LeaveSpaceAsync(spaceId);

        _mockRepo.Verify(x => x.SoulRepository.DeleteSpaceMemberAsync(existingSoul.Id, targetSpace.Id), Times.Once);
        _mockRepo.Verify(x => x.UnitOfWork.CommitAsync(), Times.Once);
    }
}
