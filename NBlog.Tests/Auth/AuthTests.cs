using Application.Dtos;
using Application.Services.Auth;
using Application.Shared;
using Domain.Entites;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace NBlog.Tests.Auth
{
    public class AuthTests
    {
        private Mock<UserManager<ApplicationUser>> _userManagerMock;
        private Mock<IValidator<RegisterDto>> _registerValidatorMock;
        private Mock<IValidator<LoginDto>> _loginValidatorMock;
        private Mock<ILogger<AuthService>> _loggerMock;
        private Mock<IOptions<JwtSettings>> _jwtSettingsMock;
        private AuthService _authService;

        public AuthTests()
        {
            InitMocks();
            InitServices();
        }

        private void InitServices()
        {
            _authService = new AuthService(
                _userManagerMock.Object,
                _loggerMock.Object,
                _registerValidatorMock.Object,
                _loginValidatorMock.Object,
                _jwtSettingsMock.Object);
        }

        private void InitMocks()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);

            _registerValidatorMock = new Mock<IValidator<RegisterDto>>();
            _loggerMock = new Mock<ILogger<AuthService>>();
            _jwtSettingsMock = new Mock<IOptions<JwtSettings>>();
            _loginValidatorMock = new Mock<IValidator<LoginDto>>();
        }

        [Theory]
        [InlineData("", "", "")]
        [InlineData("test", "test@", "test")]
        [InlineData("test", "test@email.com", "test")]
        [InlineData("", "test@email.com", "Test@1234")]
        public async Task RegisterUser_ShouldReturnError_WhenInvalidData(string username, string email, string password)
        {
            // Arrange

            var registerDto = new RegisterDto
            {
                UserName = username,
                Email = email,
                Password = password
            };


            // Create a validation result to simulate failure
            var validationResult = new ValidationResult(new List<ValidationFailure>
            {
                new ValidationFailure("Field", "Validation error") // Generic failure for simplicity
            });

            _registerValidatorMock
                .Setup(v => v.ValidateAsync(registerDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            var result = await _authService.RegisterUser(registerDto);

            Assert.False(result.IsSuccess);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);

        }


        [Fact]
        public async Task RegisterUser_ShouldReturnError_WhenEmailExists()
        {

            _userManagerMock.Setup(u => u.FindByEmailAsync(It.IsAny<string>()))
                           .ReturnsAsync(new ApplicationUser());

            _registerValidatorMock.Setup(v => v.ValidateAsync(It.IsAny<RegisterDto>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(new ValidationResult());

            var registerDto = new RegisterDto
            {
                UserName = "test",
                Email = "test@mail.com",
                Password = "Test@1234"
            };

            // Act
            var result = await _authService.RegisterUser(registerDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        }

        [Fact]
        public async Task RegisterUser_ShouldReturnSuccess_WhenValidData()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                UserName = "test",
                Email = "Test@mail.com",
                Password = "Test@1234"
            };


            _registerValidatorMock
                .Setup(v => v.ValidateAsync(registerDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _userManagerMock.Setup(u => u.FindByEmailAsync(registerDto.Email))
                .ReturnsAsync(value: null);

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

            var result = await _authService.RegisterUser(registerDto);


            Assert.True(result.IsSuccess);
            Assert.Equal(StatusCodes.Status201Created, result.StatusCode);

        }



        [Theory]
        [InlineData("", "")]
        [InlineData("test@", "test")]
        [InlineData("test@mail.com", "test")]
        public async Task LoginUser_ShouldReturnError_WhenInvalidData(string email, string password)
        {
            _loginValidatorMock.Setup(v => v.ValidateAsync(It.IsAny<LoginDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(new List<ValidationFailure>
                {
                    new ValidationFailure("Field", "Validation error")
                }));


            var loginDto = new LoginDto()
            {
                Email = email,
                Password = password
            };

            var result = await _authService.LoginUser(loginDto);

            Assert.False(result.IsSuccess);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);

        }


        [Fact]
        public async Task LoginUser_ShouldReturnError_WhenUserNotFound()
        {
            _loginValidatorMock.Setup(v => v.ValidateAsync(It.IsAny<LoginDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _userManagerMock.Setup(u => u.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(value: null);

            var loginDto = new LoginDto()
            {
                Email = "test@mail",
                Password = "Test@1234"

            };

            var result = await _authService.LoginUser(loginDto);

            Assert.False(result.IsSuccess);
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);

        }

        [Fact]
        public async Task LoginUser_ShouldReturnError_WhenPasswordIsIncorrect()
        {
            _loginValidatorMock.Setup(v => v.ValidateAsync(It.IsAny<LoginDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _userManagerMock.Setup(u => u.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(new ApplicationUser());

            _userManagerMock.Setup(u => u.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            var loginDto = new LoginDto()
            {
                Email = "test@mail",
                Password = "Test@1234"
            };

            var result = await _authService.LoginUser(loginDto);
            Assert.False(result.IsSuccess);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        }


        [Fact]
        public async Task LoginUser_ShouldReturnSuccess_WhenValidData()
        {
            _loginValidatorMock.Setup(v => v.ValidateAsync(It.IsAny<LoginDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _userManagerMock.Setup(u => u.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(new ApplicationUser());

            _userManagerMock.Setup(u => u.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(true);


            var loginDto = new LoginDto()
            {
                Email = "test@mail",
                Password = "Test@1234"
            };
            var result = await _authService.LoginUser(loginDto);

            Assert.True(result.IsSuccess);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }
    }
}



