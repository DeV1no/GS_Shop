using GS_Shop_UserManagement.Application.Contracts.Persistence;
using GS_Shop_UserManagement.Application.DTOs.FileManager;
using GS_Shop_UserManagement.Domain.Entities;
using GS_Shop_UserManagement.Persistence.FileManager.Services;
using GS_Shop_UserManagement.Persistence.Minio.Interfaces;
using GS_Shop_UserManagement.Persistence.Minio.ServiceModels;
using JWT.Serializers.Converters;
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
        private readonly IStorageService _storageService;

        public WeatherForecastController(IFileService<User> uploadService, ILogger<WeatherForecastController> logger,
            IStorageService storageService)
        {
            _uploadService = uploadService;
            _logger = logger;
            _storageService = storageService;
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
                //  var fileName = await _uploadService.PostFileAsync(fileDetails.FileDetails);
                // var fileName = await _storageService.UploadFileAsync("test",fileDetails.FileDetails);
                var uploadFileServiceModel = new UploadFileServiceModel(fileDetails.FileDetails, "jpg", "testapi");
                var fileName = await _storageService.UploadBase64FileAsync(uploadFileServiceModel);
                return Ok(fileName);
            }
            catch (Exception)
            {
                throw;
            }
        }
        

        [HttpGet("DownloadMinioFile")]
        public async Task<ActionResult> DownloadMinioFile(int id)
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


        [HttpPost("DownloadFile")]
        public async Task<ActionResult> DownloadFile(GetObjectDownloadLinkRequestModel mdl)
        {
            try
            {
                var fileName = await _storageService.GetObjectDownloadLink(mdl);
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

        [HttpPost(Name = "AddBucket")]
        public async Task<IActionResult> AddBucket(string name)
        {
            var bucketModel = new CreateBucketServiceModel(name);
            await _storageService.CreateBucketAsync(bucketModel);
            return Ok();
        } 
    }
}