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
        private readonly IUserService _service;

        public UserController( IUserService service)
        {
            _service = service;
        }

        [HttpPost("Login")]
        [ProducesResponseType(typeof(LoginResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginEvent login)
        {
            return Ok(await _service.Login(login));
        }
        
        
        [HttpPost("Register")]
        [ProducesResponseType(typeof(RegisterEvent), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterEvent register)
        {
            return Ok(await _service.Register(register));
        }
    }
}