using ESG_Survey_Automation.Domain;
using ESG_Survey_Automation.Infrastructure.FileStorage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ESG_Survey_Automation.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SurveyController : ControllerBase
    {
        private readonly IFileStorage _fileStorage;
        private readonly ILogger<SurveyController> _logger;
        private readonly IHttpClientFactory _factory;
        private readonly IConfiguration _configuration;
        public SurveyController(IFileStorage fileStorage, ILogger<SurveyController> logger, IHttpClientFactory factory, IConfiguration configuration)
        {
            _fileStorage = fileStorage;
            _logger = logger;
            _factory = factory;
            _configuration = configuration;
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<AnswerModel>> AskQuestion(string question)
        {
            if (string.IsNullOrWhiteSpace(question))
            {
                _logger.LogWarning("Invalid question");
                return BadRequest("Invalid question");
            }
            try
            {
                var client = _factory.CreateClient();
                var obj = new { question };
                _logger.LogInformation($"Asking question to AI - {question}");
                var responseMessage = await client.PostAsJsonAsync(_configuration["AiRequestPath"], obj);
                if(responseMessage.IsSuccessStatusCode)
                {
                    string json = await responseMessage.Content.ReadAsStringAsync();
                    _logger.LogInformation($"AI answered successfully");
                    return Ok(JsonConvert.DeserializeObject<AnswerModel>(json));
                }
                else
                {
                    _logger.LogCritical($"AI failed to answer question - {question}");
                    return StatusCode((int)responseMessage.StatusCode, "Unable to process");
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Unable to ask question now..");
            }
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> UploadSurveyQuestionAir(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning($"File was not uploaded - Bad request");
                return BadRequest("No file uploaded.");
            }
            try
            {
                await _fileStorage.UploadFileToCloud(file);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Unable to upload file to Cloud storage");
                return StatusCode(StatusCodes.Status500InternalServerError, "Unable to upload file please try later");
            }
            _logger.LogInformation($"File {file.Name} was uploaded - Bad request");
            return Ok("File uploaded successfully.");
        }
    }
}
