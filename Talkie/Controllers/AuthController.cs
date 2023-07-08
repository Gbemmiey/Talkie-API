using Microsoft.AspNetCore.Mvc;
using Talkie.DTOs.Account;
using Talkie.Services.Auth;

namespace Talkie.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepo;

        public AuthController(IAuthRepository authRepo)
        {
            _authRepo = authRepo;
        }

        [HttpPost("register")]
        public async Task<ActionResult<ServiceResponse<long>>> Register(AddAccountDto request)
        {
            var response = await _authRepo.Register(request);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<ActionResult<ServiceResponse<string>>> Login(LoginAccountDto request)
        {
            var response = await _authRepo.Login(request.EmailAddress, request.Password);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}