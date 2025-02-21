using Application.Dtos;
using Application.Services.Auth;
using Application.Shared;
using Domain.Entites;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NBlog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        public AuthController(AuthService userService)
        {
            _authService = userService;
        }

        [HttpGet("all")]
        public async Task<IEnumerable<UsersDto>> GetAllUsers()
        {
            return await _authService.GetAllUsers();
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser(RegisterDto registerDto)
        {
            var result =  await _authService.RegisterUser(registerDto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUser(LoginDto loginDto)
        {
            var result = await _authService.LoginUser(loginDto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var result = await _authService.DeleteUser(id);
            return StatusCode(result.StatusCode, result);
        }

    }
}
