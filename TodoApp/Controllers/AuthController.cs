using Microsoft.AspNetCore.Mvc;
using static TodoApp.DTOs.UserDto;
using TodoApp.Services;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using TodoApp.Data;
using TodoApp.Models;
using Microsoft.EntityFrameworkCore;


namespace TodoApp.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthController : ControllerBase
	{
		private readonly AppDbContext _context;
		private readonly IConfiguration _config;

		public AuthController(AppDbContext context, IConfiguration config)
		{
			_context = context;
			_config = config;
		}

		[HttpPost("signup")]
		public async Task<IActionResult> Signup(RegisterDto dto)
		{
			if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
				return BadRequest("Username is already taken.");

			var user = new User
			{
				Id = Guid.NewGuid(),
				Username = dto.Username,
				PasswordHash = HashPassword(dto.Password)
			};

			_context.Users.Add(user);
			await _context.SaveChangesAsync();

			return Ok(new { user.Id, user.Username });
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login(LoginDto dto)
		{
			var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
			if (user == null || !VerifyPassword(dto.Password, user.PasswordHash))
				return Unauthorized("Invalid credentials");

			var token = GenerateJwtToken(user);
			return Ok(new { token, userId = user.Id, username = user.Username });
		}

		private string HashPassword(string password)
		{
			byte[] salt = Encoding.UTF8.GetBytes("static-salt-123"); // Replace with a real salt mechanism in production
			return Convert.ToBase64String(KeyDerivation.Pbkdf2(
				password, salt, KeyDerivationPrf.HMACSHA256, 10000, 32));
		}

		private bool VerifyPassword(string password, string hash)
		{
			return HashPassword(password) == hash;
		}

		private string GenerateJwtToken(User user)
		{
			var jwt = _config.GetSection("Jwt");
			var claims = new[]
			{
			new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
			new Claim(ClaimTypes.Name, user.Username)
		};

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer: jwt["Issuer"],
				audience: jwt["Audience"],
				claims: claims,
				expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwt["ExpiresInMinutes"])),
				signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}

}
