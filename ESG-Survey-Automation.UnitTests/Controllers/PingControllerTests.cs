using ESG_Survey_Automation.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESG_Survey_Automation.UnitTests.Controllers
{
    public class PingControllerTests
    {
        [Fact]
        public void Ping_ReturnsOkWithPong()
        {
            // Arrange
            var controller = new PingController();

            // Act
            var result = controller.Ping();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Pong", okResult.Value);
        }
    }
}
