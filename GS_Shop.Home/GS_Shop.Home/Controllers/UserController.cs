using System.Net;
using EventBus.Messages.Events;
using GS_Shop.Home.Services.IServices;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace GS_Shop.Home.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IRequestClient<LoginEvent> _requestClient;
        private readonly IUserService _service;

        public UserController(IRequestClient<LoginEvent> requestClient, IUserService service)
        {
            _requestClient = requestClient;
            _service = service;
        }

        [HttpPost]
        [ProducesResponseType(typeof(LoginResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginEvent login)
        {
            return Ok(await _service.Login(login));
        }
    }
}