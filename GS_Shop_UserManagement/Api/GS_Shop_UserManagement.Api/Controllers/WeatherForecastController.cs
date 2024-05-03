using GS_Shop_UserManagement.Application.Contracts.Persistence;
using GS_Shop_UserManagement.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GS_Shop_UserManagement.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IFileService<User> _uploadService;
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IDownloadStorageService _storageService;

        public WeatherForecastController(IFileService<User> uploadService, ILogger<WeatherForecastController> logger,
            IDownloadStorageService storageService)
        {
            _uploadService = uploadService;
            _logger = logger;
            _storageService = storageService;
        }

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };


        [HttpGet("DownloadMinioFile")]
        [Authorize(AuthenticationSchemes = "Bearer", Policy = "UpdateUserPolicy")]
        public async Task<ActionResult> DownloadMinioFile(string downloadLink)
        {
            var fileName = await _storageService.GetObjectDownloadLink(downloadLink);
            return Ok(fileName);
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