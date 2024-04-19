using System.Net;
using EventBus.Messages.Events;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace GS_Shop.Home.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IRequestClient<LoginEvent> _requestClient;

        public UserController(IRequestClient<LoginEvent> requestClient)
        {
            _requestClient = requestClient;
        }

        [HttpPost]
        [ProducesResponseType(typeof(LoginResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginEvent login)
        {
            try
            {
                var response = await _requestClient.GetResponse<LoginResponse>(login);

                // Return the response from the consumer service to the client
                return Ok(response.Message);
            }
            catch (RequestTimeoutException)
            {
                // Handle request timeout
                return BadRequest("Request timeout occurred.");
            }
            catch (Exception ex)
            {
                // Log the exception
                return BadRequest("Failed to process login: " + ex.Message);
            }
        }
    }
}