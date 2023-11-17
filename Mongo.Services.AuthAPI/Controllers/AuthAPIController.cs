using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mongo.Services.AuthAPI.Model.Dto;
using Mongo.Services.AuthAPI.Service.IService;

namespace Mongo.Services.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]

    public class AuthAPIController : ControllerBase
    {
        private readonly IAuthService _authService;
        protected ResponseDto _response;

        public AuthAPIController(IAuthService authService)
        {
            _authService = authService;
            _response = new(); 
        }

        [HttpPost("register")]
        public async Task<ActionResult> Reg([FromBody] RegisterRequestDto model)
        {
            var errorMessage = await _authService.Register(model);
            if(!string.IsNullOrEmpty(errorMessage))
            {
                _response.IsSuccessful = false;
                _response.message = errorMessage;
                return BadRequest(_response);
            }
            return Ok(_response);
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginRequestDto login)
        {
            LoginResponseDto loginResponse = await _authService.Login(login);
            if (loginResponse.user == null || string.IsNullOrEmpty(loginResponse.token))
            {
                _response.IsSuccessful = false;
                _response.message = "Invalid Login Credential";
                return BadRequest(loginResponse);
            }
            return Ok(loginResponse);
        }

        [HttpPost("AssignRole")]
        public async Task<ActionResult> AssignRole([FromBody] RegisterRequestDto model)
        {
            bool isRoleAssign = await _authService.AssignRole(model.email, model.role);
            if (!isRoleAssign)
            {
                _response.IsSuccessful = false;
                _response.message = "unable to create role";
                return BadRequest(_response);
            }
            return Ok(_response);
        }
    }
}
