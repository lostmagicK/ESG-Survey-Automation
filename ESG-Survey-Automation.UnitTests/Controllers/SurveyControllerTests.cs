using ESG_Survey_Automation.Controllers;
using Microsoft.Extensions.Configuration;
using ESG_Survey_Automation.Infrastructure.FileStorage;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;
using ESG_Survey_Automation.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Newtonsoft.Json;

namespace ESG_Survey_Automation.UnitTests.Controllers
{
    public class SurveyControllerTests : ESGSurveyContextTestBase
    {
        private readonly SurveyController _controller;
        private readonly Mock<IFileStorage> _fileStorageMock;
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
        private readonly IConfiguration _configuration;
        private readonly Mock<ILogger<SurveyController>> _loggerMock;
        public SurveyControllerTests()
        {
            _configuration = new ConfigurationBuilder().AddInMemoryCollection(IConfigurationTest.configValues).Build();
            _fileStorageMock = new Mock<IFileStorage>();
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _loggerMock = new Mock<ILogger<SurveyController>>();
            _controller = new SurveyController(_fileStorageMock.Object, _loggerMock.Object, _httpClientFactoryMock.Object, _configuration);
        }


        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task AskQuestion_InvalidQuestion_ReturnsBadRequest(string question)
        {
            // Act
            var result = await _controller.AskQuestion(question);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid question", badRequestResult.Value);
        }


        [Fact]
        public async Task UploadSurveyQuestionAir_ValidFile_ReturnsOk()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.SetupGet(f => f.Length).Returns(10); // Simulate a file with length
            _fileStorageMock.Setup(x => x.UploadFileToCloud(It.IsAny<IFormFile>(), It.IsAny<string>())).Verifiable();

            // Act
            var result = await _controller.UploadSurveyQuestionAir(fileMock.Object);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("File uploaded successfully.", okResult.Value);
            _fileStorageMock.Verify(x => x.UploadFileToCloud(fileMock.Object, "SurveyQuestionair"), Times.Once);
        }

        [Fact]
        public async Task UploadSurveyQuestionAir_NullFile_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.UploadSurveyQuestionAir(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("No file uploaded.", badRequestResult.Value);
            _fileStorageMock.Verify(x => x.UploadFileToCloud(It.IsAny<IFormFile>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task UploadTraingDocument_ValidFile_ReturnsOk()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.SetupGet(f => f.Length).Returns(10); // Simulate a file with length
            _fileStorageMock.Setup(x => x.UploadFileToCloud(It.IsAny<IFormFile>(), It.IsAny<string>())).Verifiable();

            // Act
            var result = await _controller.UploadTraingDocument(fileMock.Object);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("File uploaded successfully.", okResult.Value);
            _fileStorageMock.Verify(x => x.UploadFileToCloud(fileMock.Object, "TrainingDocument"), Times.Once);
        }

        [Fact]
        public async Task UploadTraingDocument_NullFile_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.UploadTraingDocument(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("No file uploaded.", badRequestResult.Value);
            _fileStorageMock.Verify(x => x.UploadFileToCloud(It.IsAny<IFormFile>(), It.IsAny<string>()), Times.Never);
        }
    }

}

