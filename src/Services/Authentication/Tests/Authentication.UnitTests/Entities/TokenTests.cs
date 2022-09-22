﻿using Authentication.Core.Contracts;
using Authentication.Core.Exceptions;
using Authentication.Core.Models;
using Moq;

namespace Authentication.UnitTests.Entities;

public class TokenTests
{
    private readonly Mock<IRepositoryManager> _mockRepo;

    public TokenTests()
    {
        Mock<IUserRepository> mockUserRepo = new();
        Mock<ITokenRepository> mockTokenRepo = new();
        _mockRepo = new Mock<IRepositoryManager>();
        _mockRepo.Setup(x => x.UserRepository).Returns(mockUserRepo.Object);
        _mockRepo.Setup(x => x.TokenRepository).Returns(mockTokenRepo.Object);
    }

    [Fact]
    public async Task RefreshAsync_RepositoryManagerIsNull_ThrowsNullReferenceException()
    {
        Token oldToken = new();

        await Assert.ThrowsAsync<NullReferenceException>(() => oldToken.RefreshAsync());
    }

    [Fact]
    public async Task RefreshAsync_TokenDoesNotExist_ThrowsInvalidRefreshTokenException()
    {
        Token oldToken = new(_mockRepo.Object);

        _mockRepo.Setup(x => x.RefreshTokenRepository.GetByOldRefreshTokenAsync(It.IsAny<string>()))
            .ReturnsAsync((RefreshToken)null!);

        await Assert.ThrowsAsync<InvalidRefreshTokenException>(() => oldToken.RefreshAsync());
    }

    [Fact]
    public async Task RefreshAsync_UserDoesNotExist_ThrowsInvalidRefreshTokenException()
    {
        Token oldToken = new(_mockRepo.Object);

        _mockRepo.Setup(x => x.RefreshTokenRepository.GetByOldRefreshTokenAsync(It.IsAny<string>()))
            .ReturnsAsync(new RefreshToken());
        _mockRepo.Setup(x => x.UserRepository.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((User)null!);

        await Assert.ThrowsAsync<InvalidRefreshTokenException>(() => oldToken.RefreshAsync());
    }

    [Fact]
    public async Task RefreshAsync_ExistingTokenIsInvalid_ThrowsInvalidRefreshTokenException()
    {
        Token oldToken = new(_mockRepo.Object);

        _mockRepo.Setup(x => x.RefreshTokenRepository.GetByOldRefreshTokenAsync(It.IsAny<string>()))
            .ReturnsAsync(new RefreshToken());
        _mockRepo.Setup(x => x.UserRepository.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new User());

        await Assert.ThrowsAsync<InvalidRefreshTokenException>(() => oldToken.RefreshAsync());
    }

    [Fact]
    public async Task RefreshAsync_ExistingTokenIsValid_ReturnsNewToken()
    {
        Token oldToken = new(_mockRepo.Object) { Value = "old-token", RefreshToken = "old-refresh-token" };

        _mockRepo.Setup(x => x.RefreshTokenRepository.GetByOldRefreshTokenAsync(It.IsAny<string>()))
            .ReturnsAsync(new RefreshToken());
        _mockRepo.Setup(x => x.UserRepository.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new User());
        _mockRepo.Setup(x => x.TokenRepository.RefreshJwtAsync(It.IsAny<Token>(), It.IsAny<User>(), It.IsAny<RefreshToken>()))
            .ReturnsAsync(new Token { Value = "new-token", RefreshToken = "new-refresh-token" });

        await oldToken.RefreshAsync();

        Assert.IsType<Token>(oldToken);
        Assert.Equal("new-token", oldToken.Value);
        Assert.Equal("new-refresh-token", oldToken.RefreshToken);
    }
}
