namespace Authentication.UnitTests.Services;

public class TokenServiceTests
{
    //private readonly Mock<IRepositoryManager> _mockRepo;
    //private readonly Mock<IConfiguration> _mockConfig;

    //public TokenServiceTests()
    //{
    //    _mockRepo = new Mock<IRepositoryManager>();
    //    Mock<IUserRepository> mockApplicationUserRepo = new();
    //    Mock<IRefreshTokenRepository> mockRefreshTokenRepo = new();
    //    _mockRepo.SetupGet(x => x.ApplicationUserRepository).Returns(mockApplicationUserRepo.Object);
    //    _mockRepo.SetupGet(x => x.RefreshTokenRepository).Returns(mockRefreshTokenRepo.Object);

    //    _mockConfig = new Mock<IConfiguration>();
    //    _mockConfig.Setup(x => x["JWT:ExpirationInMinutes"]).Returns("60");
    //    _mockConfig.Setup(x => x["JWT:Secret"]).Returns("this-is-just-a-fake-key");
    //    _mockConfig.Setup(x => x["JWT:Issuer"]).Returns("https://fakeissuer.com");
    //    _mockConfig.Setup(x => x["JWT:Audience"]).Returns("fake-audience");
    //    _mockConfig.Setup(x => x["JWT:RefreshTokenExpirationInMonths"]).Returns("6");

    //    _tokenService = new TokenService(_mockRepo.Object, _mockConfig.Object, TokenData.GetTokenValidationParameters(_mockConfig.Object));
    //}

    //[Theory]
    //[InlineData("")]
    //[InlineData(null)]
    //[InlineData("testemail.com")]
    //[InlineData("test@emailcom")]
    //public async Task GenerateJwtAsync_EmailIsInvalid_ThrowsInvalidEmailException(string email)
    //{
    //    UserDto user = new() { Email = email };

    //    await Assert.ThrowsAsync<InvalidEmailException>(() => _tokenService.GenerateJwtAsync(user));
    //}

    //[Fact]
    //public async Task GenerateJwtAsync_RefreshTokenIsNotNull_ReturnsNewTokenWithoutInsertingNewItemInTheDatabase()
    //{
    //    UserDto user = new()
    //    {
    //        Id = Guid.NewGuid(),
    //        Email = "test@example.com",
    //        FirstName = "First",
    //        LastName = "Last"
    //    };

    //    var result = await _tokenService.GenerateJwtAsync(user, new RefreshTokenDto());

    //    _mockRepo.Verify(x => x.RefreshTokenRepository.CreateAsync(It.IsAny<RefreshToken>()), Times.Never);
    //    Assert.IsType<TokenDto>(result);
    //}

    //[Fact]
    //public async Task GenerateJwtAsync_RefreshTokenIsNull_ReturnsNewTokenAndInsertNewItemInTheDatabase()
    //{
    //    UserDto user = new()
    //    {
    //        Id = Guid.NewGuid(),
    //        Email = "test@example.com",
    //        FirstName = "First",
    //        LastName = "Last"
    //    };

    //    var result = await _tokenService.GenerateJwtAsync(user);

    //    _mockRepo.Verify(x => x.RefreshTokenRepository.CreateAsync(It.IsAny<RefreshToken>()), Times.Once);
    //    Assert.IsType<TokenDto>(result);
    //}

    //[Fact]
    //public async Task RefreshJwtAsync_TokenDoesNotExist_ThrowsInvalidRefreshTokenException()
    //{
    //    _mockRepo.Setup(x => x.RefreshTokenRepository.GetByOldRefreshTokenAsync(It.IsAny<string>()))
    //        .ReturnsAsync((RefreshToken)null!);

    //    await Assert.ThrowsAsync<InvalidRefreshTokenException>(() => _tokenService.RefreshJwtAsync(new TokenDto()));
    //}

    //[Fact]
    //public async Task RefreshJwtAsync_ApplicationUserDoesNotExist_ThrowsInvalidRefreshTokenException()
    //{
    //    _mockRepo.Setup(x => x.RefreshTokenRepository.GetByOldRefreshTokenAsync(It.IsAny<string>()))
    //        .ReturnsAsync(new RefreshToken());
    //    _mockRepo.Setup(x => x.ApplicationUserRepository.GetByIdAsync(It.IsAny<string>()))
    //        .ReturnsAsync((User)null!);

    //    await Assert.ThrowsAsync<InvalidRefreshTokenException>(() => _tokenService.RefreshJwtAsync(new TokenDto()));
    //}

    //[Fact]
    //public async Task RefreshJwtAsync_ExistingTokenIsInvalid_ThrowsInvalidRefreshTokenException()
    //{
    //    _mockRepo.Setup(x => x.RefreshTokenRepository.GetByOldRefreshTokenAsync(It.IsAny<string>()))
    //        .ReturnsAsync(new RefreshToken());
    //    _mockRepo.Setup(x => x.ApplicationUserRepository.GetByIdAsync(It.IsAny<string>()))
    //        .ReturnsAsync(new User());

    //    await Assert.ThrowsAsync<InvalidRefreshTokenException>(() => _tokenService.RefreshJwtAsync(new TokenDto()));
    //}

    //[Fact]
    //public async Task RefreshJwtAsync_ExistingTokenIsValid_ReturnsNewToken()
    //{
    //    UserDto user = new()
    //    {
    //        Id = Guid.NewGuid(),
    //        Email = "test@example.com",
    //        FirstName = "First",
    //        LastName = "Last"
    //    };
    //    _mockRepo.Setup(x => x.RefreshTokenRepository.GetByOldRefreshTokenAsync(It.IsAny<string>()))
    //        .ReturnsAsync(new RefreshToken());
    //    _mockRepo.Setup(x => x.ApplicationUserRepository.GetByIdAsync(It.IsAny<string>()))
    //        .ReturnsAsync(new User
    //        {
    //            Id = user.Id,
    //            Email = user.Email,
    //            FirstName = user.FirstName,
    //            LastName = user.LastName
    //        });
    //    var token = await _tokenService.GenerateJwtAsync(user);

    //    var result = await _tokenService.RefreshJwtAsync(token);

    //    Assert.IsType<TokenDto>(result);
    //}
}
