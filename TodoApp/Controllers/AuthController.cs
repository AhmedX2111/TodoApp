using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static TodoApp.DTOs.UserDto;
using TodoApp.Services;

namespace TodoApp.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly AuthService _authService;

		public AuthController(AuthService authService)
		{
			_authService = authService;
		}

		[HttpPost("signup")]
		public async Task<IActionResult> Signup(RegisterDto dto)
		{
			if (!await _authService.RegisterAsync(dto.Username, dto.Password))
				return BadRequest("User already exists");

			return Ok("User registered");
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login(LoginDto dto)
		{
			var token = await _authService.LoginAsync(dto.Username, dto.Password);
			if (token == null) return Unauthorized("Invalid credentials");

			return Ok(new { token });
		}

	}
}
