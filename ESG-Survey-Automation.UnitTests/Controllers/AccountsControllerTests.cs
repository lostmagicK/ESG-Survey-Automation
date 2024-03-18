using ESG_Survey_Automation.Controllers;
using ESG_Survey_Automation.Domain;
using ESG_Survey_Automation.Infrastructure.EntityFramework.Models;
using ESG_Survey_Automation.Infrastructure.EntityFramework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;


namespace ESG_Survey_Automation.UnitTests.Controllers
{
    public class AccountsControllerTests : ESGSurveyContextTestBase
    {
        private readonly AccountsController _controller;
        private readonly Mock<ESGSurveyContext> _contextMock;
        private readonly IConfiguration _configuration;
        private readonly Mock<ILogger<AccountsController>> _loggerMock;

        public AccountsControllerTests()
        {
            _configuration = new ConfigurationBuilder() .AddInMemoryCollection(IConfigurationTest.configValues) .Build();
            _loggerMock = new Mock<ILogger<AccountsController>>();
            _controller = new AccountsController(_context, _configuration, _loggerMock.Object);
        }

        [Fact]
        public async Task SignIn_ValidUser_ReturnsOk()
        {
            // Arrange
            var userModel = new LoginModel { Email = "test@example.com", Password = "a" };
            var salt = BCrypt.Net.BCrypt.GenerateSalt();
            var pass = $"{{bcrypt}}{BCrypt.Net.BCrypt.HashPassword(userModel.Password, salt)}";
            AddEntityToDatabase(new User
            {
                FullName = "Test User",
                RegistrationDate = DateTime.UtcNow,
                UserId = Guid.NewGuid(),
                Email = userModel.Email,
                EncryptedPassword = pass
            });

            // Act
            var result = await _controller.SignIn(userModel);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
            Assert.IsType<LoginResponse>(((OkObjectResult)result.Result).Value);
        }

        [Fact]
        public async Task SignIn_InvalidUser_ReturnsNotFound()
        {
            // Arrange
            var userModel = new LoginModel { Email = "test@example.com", Password = "password" };

            // Act
            var result = await _controller.SignIn(userModel);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task SignIn_IncorrectPassword_ReturnsUnauthorized()
        {
            // Arrange
            var userModel = new LoginModel { Email = "test@example.com", Password = "b" };
            var salt = BCrypt.Net.BCrypt.GenerateSalt();
            var pass = $"{{bcrypt}}{BCrypt.Net.BCrypt.HashPassword("a", salt)}";
            AddEntityToDatabase(new User
            {
                FullName = "Test User",
                RegistrationDate = DateTime.UtcNow,
                UserId = Guid.NewGuid(),
                Email = userModel.Email,
                EncryptedPassword = pass
            });
            // Act
            var result = await _controller.SignIn(userModel);

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(result.Result);
        }

        [Fact]
        public async Task Registration_NewUser_ReturnsOk()
        {
            // Arrange
            var registrationModel = new UserRegistrationModel { Email = "newuser@example.com", Password = "a", FullName = "New User" };

            // Act
            var result = await _controller.Registration(registrationModel);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task Registration_ExistingUser_ReturnsBadRequest()
        {
            // Arrange
            var existingUserEmail = "existing@example.com";
            var registrationModel = new UserRegistrationModel { Email = existingUserEmail, Password = "password", FullName = "Existing User" };
            AddEntityToDatabase(new User
            {
                FullName = registrationModel.FullName,
                RegistrationDate = DateTime.UtcNow,
                UserId = Guid.NewGuid(),
                Email = existingUserEmail,
                EncryptedPassword = ""
            });

            // Act
            var result = await _controller.Registration(registrationModel);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task EmailExists_ExistingEmail_ReturnsOkWithTrue()
        {
            // Arrange
            var existingEmail = "existing@example.com";
            AddEntityToDatabase(new User
            {
                FullName = "Test User",
                RegistrationDate = DateTime.UtcNow,
                UserId = Guid.NewGuid(),
                Email = existingEmail,
                EncryptedPassword = ""
            });

            // Act
            var result = await _controller.EmailExists(existingEmail);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
            Assert.True((bool)((OkObjectResult)result.Result).Value);
        }

        [Fact]
        public async Task EmailExists_NonExistingEmail_ReturnsOkWithFalse()
        {
            // Arrange
            var nonExistingEmail = "nonexisting@example.com";

            // Act
            var result = await _controller.EmailExists(nonExistingEmail);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
            Assert.False((bool)((OkObjectResult)result.Result).Value);
        }
    }
}
