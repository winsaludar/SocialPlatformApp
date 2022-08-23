namespace Authentication.UnitTests.Services;

public class ApplicationUserServiceTests
{
    //private readonly Mock<IRepositoryManager> _mockRepo;

    //public ApplicationUserServiceTests()
    //{
    //    _mockRepo = new Mock<IRepositoryManager>();
    //    Mock<IUserRepository> mockApplicationUserRepo = new();
    //    _mockRepo.SetupGet(x => x.ApplicationUserRepository).Returns(mockApplicationUserRepo.Object);
    //}

    //[Theory]
    //[InlineData("")]
    //[InlineData(null)]
    //[InlineData("testemail.com")]
    //[InlineData("test@emailcom")]
    //public async Task GetByEmailAsync_EmailIsInvalid_ThrowsInvalidEmailException(string email) =>
    //    await Assert.ThrowsAsync<InvalidEmailException>(() => _applicationUserService.GetByEmailAsync(email));

    //[Fact]
    //public async Task GetByEmailAsync_EmailDoesNotExist_ThrowsUserNotFoundException()
    //{
    //    string email = "notexistinguser@email.com";
    //    _mockRepo.Setup(x => x.ApplicationUserRepository.GetByEmailAsync(email))
    //        .ReturnsAsync((User)null!);

    //    await Assert.ThrowsAsync<UserNotFoundException>(() => _applicationUserService.GetByEmailAsync(email));
    //}

    //[Fact]
    //public async Task GetByEmailAsync_EmailIsValidAndDoesExist_ReturnsApplicationUserDto()
    //{
    //    string email = "existinguser@email.com";
    //    _mockRepo.Setup(x => x.ApplicationUserRepository.GetByEmailAsync(email))
    //        .ReturnsAsync(new User { Email = email });

    //    var result = await _applicationUserService.GetByEmailAsync(email);

    //    Assert.IsType<UserDto>(result);
    //    Assert.Equal(email, result.Email);
    //}

    //[Theory]
    //[InlineData("")]
    //[InlineData(null)]
    //[InlineData("testemail.com")]
    //[InlineData("test@emailcom")]
    //public async Task RegisterAsync_EmailIsInvalid_ThrowsInvalidEmailException(string email)
    //{
    //    RegisterUserDto newUser = new()
    //    {
    //        FirstName = "First",
    //        LastName = "Last",
    //        Email = email,
    //        Password = "password"
    //    };

    //    await Assert.ThrowsAsync<InvalidEmailException>(() => _applicationUserService.RegisterAsync(newUser));
    //}

    //[Fact]
    //public async Task RegisterAsync_EmailAlreadyExist_ThrowsUserAlreadyExistException()
    //{
    //    string email = "existinguser@email.com";
    //    RegisterUserDto newUser = new()
    //    {
    //        FirstName = "First",
    //        LastName = "Last",
    //        Email = email,
    //        Password = "password"
    //    };
    //    _mockRepo.Setup(x => x.ApplicationUserRepository.GetByEmailAsync(email))
    //        .ReturnsAsync(new User { Email = email });

    //    await Assert.ThrowsAsync<UserAlreadyExistException>(() => _applicationUserService.RegisterAsync(newUser));
    //}

    //[Fact]
    //public async Task RegisterAsync_PasswordIsInvalid_ThrowsInvalidPasswordException()
    //{
    //    RegisterUserDto newUser = new()
    //    {
    //        FirstName = "First",
    //        LastName = "Last",
    //        Email = "test@example.com",
    //        Password = ""
    //    };

    //    _mockRepo.Setup(x => x.ApplicationUserRepository.ValidateRegistrationPassword(newUser.Password))
    //        .ReturnsAsync(false);

    //    await Assert.ThrowsAsync<InvalidPasswordException>(() => _applicationUserService.RegisterAsync(newUser));
    //}

    //[Fact]
    //public async Task RegisterAsync_EmailAndPasswordAreBothValid_CreateUser()
    //{
    //    User? createdUser = null;
    //    string email = "nonexistinguser@email.com";
    //    string password = "P@$$w0rd1";
    //    _mockRepo.Setup(x => x.ApplicationUserRepository.GetByEmailAsync(email))
    //        .ReturnsAsync((User)null!);
    //    _mockRepo.Setup(x => x.ApplicationUserRepository.ValidateRegistrationPassword(password))
    //        .ReturnsAsync(true);
    //    _mockRepo.Setup(x => x.ApplicationUserRepository.RegisterAsync(It.IsAny<User>(), password))
    //        .Callback<User, string>((x, y) => createdUser = x);
    //    RegisterUserDto newUser = new()
    //    {
    //        FirstName = "First",
    //        LastName = "Last",
    //        Email = email,
    //        Password = password
    //    };

    //    await _applicationUserService.RegisterAsync(newUser);

    //    _mockRepo.Verify(x => x.ApplicationUserRepository.RegisterAsync(It.IsAny<User>(), password), Times.Once);
    //    Assert.Equal(createdUser?.FirstName, newUser.FirstName);
    //    Assert.Equal(createdUser?.LastName, newUser.LastName);
    //    Assert.Equal(createdUser?.Email, newUser.Email);
    //}

    //[Theory]
    //[InlineData("")]
    //[InlineData(null)]
    //[InlineData("testemail.com")]
    //[InlineData("test@emailcom")]
    //public async Task LoginAsync_EmailIsInvalid_ThrowsInvalidEmailException(string email)
    //{
    //    LoginUserDto user = new()
    //    {
    //        Email = email,
    //        Password = "password"
    //    };

    //    await Assert.ThrowsAsync<InvalidEmailException>(() => _applicationUserService.LoginAsync(user));
    //}

    //[Fact]
    //public async Task LoginAsync_EmailDoesNotExist_ThrowsUnauthorizedAccessException()
    //{
    //    string email = "nonexistingemail@example.com";
    //    LoginUserDto user = new() { Email = email, Password = "password" };
    //    _mockRepo.Setup(x => x.ApplicationUserRepository.GetByEmailAsync(email))
    //        .ReturnsAsync((User)null!);

    //    await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _applicationUserService.LoginAsync(user));
    //}

    //[Fact]
    //public async Task LoginAsync_EmailDoesExistButPasswordIsIncorrect_ThrowsUnauthorizedAccessException()
    //{
    //    string email = "existingemail@example.com";
    //    string password = "incorrect-password";
    //    LoginUserDto user = new() { Email = email, Password = password };
    //    _mockRepo.Setup(x => x.ApplicationUserRepository.GetByEmailAsync(email))
    //        .ReturnsAsync(new User());
    //    _mockRepo.Setup(x => x.ApplicationUserRepository.ValidateLoginPassword(email, password))
    //        .ReturnsAsync(false);

    //    await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _applicationUserService.LoginAsync(user));
    //}

    //[Fact]
    //public async Task LoginAsync_EmailAndPasswordAreBothCorrect_ReturnsTokenObject()
    //{
    //    string email = "existingemail@example.com";
    //    string password = "correct-password";
    //    LoginUserDto user = new() { Email = email, Password = password };
    //    _mockRepo.Setup(x => x.ApplicationUserRepository.GetByEmailAsync(email))
    //        .ReturnsAsync(new User
    //        {
    //            Id = Guid.NewGuid(),
    //            FirstName = "First",
    //            LastName = "Last",
    //            Email = email
    //        });
    //    _mockRepo.Setup(x => x.ApplicationUserRepository.ValidateLoginPassword(email, password))
    //        .ReturnsAsync(true);
    //    _mockTokenService.Setup(x => x.GenerateJwtAsync(It.IsAny<UserDto>(), null))
    //        .ReturnsAsync(new TokenDto { Token = "fake-token", RefreshToken = "fake-refresh-token" });

    //    var result = await _applicationUserService.LoginAsync(user);

    //    Assert.IsType<TokenDto>(result);
    //    Assert.NotEmpty(result.Token);
    //}
}
