using ESG_Survey_Automation.Domain;
using ESG_Survey_Automation.Infrastructure.EntityFramework;
using ESG_Survey_Automation.Infrastructure.EntityFramework.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ESG_Survey_Automation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ESGSurveyContext _context;
        private readonly ILogger _logger;
        public AccountsController(ESGSurveyContext context, IConfiguration configuration, ILogger<AccountsController> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<ActionResult<LoginResponse>> SignIn([FromBody] LoginModel model)
        {
            var user = await _context.Users.Where(x => x.Email == model.Email).FirstOrDefaultAsync();
            if (user == null)
            {
                string error = $"User not found with email {model.Email}";
                _logger.LogWarning(error);
                return NotFound(error);
            }
            if (BCrypt.Net.BCrypt.Verify(model.Password, user.EncryptedPassword[8..]))
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration.GetValue<string>("JwtOptions:SecurityKey"));
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Issuer = _configuration.GetValue<string>("JwtOptions:Issuer"),
                    Audience = _configuration.GetValue<string>("JwtOptions:Audience"),
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, user.FullName),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    }),
                    Expires = DateTime.UtcNow.AddDays(14),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                LoginResponse response = new()
                {
                    AccessToken = tokenHandler.WriteToken(token),
                    TokenType = "Bearer",
                    UserName = user.FullName
                };
                _logger.LogInformation($"User {model.Email} has logined successfully");
                return Ok(response);
            }
            _logger.LogWarning($"User {model.Email} has entered wrong password");
            return Unauthorized("Invalid password");
        }

        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<ActionResult> Registration([FromBody] UserRegistrationModel model)
        {
            var salt = BCrypt.Net.BCrypt.GenerateSalt();
            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
            {
                string error = $"User with email {model.Email} already exist";
                _logger.LogWarning(error);
                return BadRequest(error);
            }
            _context.Users.Add(new User
            {
                Email = model.Email,
                EncryptedPassword = $"{{bcrypt}}{BCrypt.Net.BCrypt.HashPassword(model.Password, salt)}",
                FullName = model.FullName,
                RegistrationDate = DateTime.UtcNow,
                UserId = Guid.NewGuid(),
            });
            await _context.SaveChangesAsync();
            _logger.LogInformation($"User {model.Email} registered successfully");
            return Ok();
        }
    }
}
