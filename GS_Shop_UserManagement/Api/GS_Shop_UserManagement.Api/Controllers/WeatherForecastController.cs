using GS_Shop_UserManagement.Persistence.FileManager.Models;
using GS_Shop_UserManagement.Persistence.FileManager.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GS_Shop_UserManagement.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IFileService _uploadService;
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(IFileService uploadService, ILogger<WeatherForecastController> logger)
        {
            _uploadService = uploadService;
            _logger = logger;
        }
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };



        [HttpPost("PostSingleFile")]
        public async Task<ActionResult> PostSingleFile([FromForm] FileUploadModel fileDetails)
        {
            if (fileDetails == null)
            {
                return BadRequest();
            }

            try
            {
                var fileName = await _uploadService.PostFileAsync(fileDetails.FileDetails);
                return Ok(fileName);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("DownloadFile")]
        public async Task<ActionResult> DownloadFile(int id)
        {
            if (id < 1)
            {
                return BadRequest();
            }

            try
            {
                var fileName = await _uploadService.DownloadFileById(id);
                return Ok(fileName);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpGet(Name = "GetWeatherForecast")]
        [Authorize(AuthenticationSchemes = "Bearer", Policy = "GetWeatherForecastPolicy")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
