using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TodoApp.Data;
using TodoApp.Models;

namespace TodoApp.Services
{
	public class AuthService
	{
		private readonly AppDbContext _context;
		private readonly PasswordHasher<User> _hasher = new();
		private readonly IConfiguration _config;

		public AuthService(AppDbContext context, IConfiguration config)
		{
			_context = context;
			_config = config;
		}

		public async Task<bool> RegisterAsync(string username, string password)
		{
			if (await _context.Users.AnyAsync(u => u.Username == username)) return false;

			var user = new User { Username = username, Id = Guid.NewGuid() }; // Generate UserId
			user.PasswordHash = _hasher.HashPassword(user, password);

			_context.Users.Add(user);
			await _context.SaveChangesAsync();

			return true;
		}


		public async Task<User?> LoginAsync(string username, string password)
		{
			// Find the user by username
			var user = await _context.Users
									 .FirstOrDefaultAsync(u => u.Username == username);

			// If the user does not exist, return null
			if (user == null)
			{
				return null; // User not found
			}

			// Check if the provided password matches the stored password hash
			var passwordResult = _hasher.VerifyHashedPassword(null, user.PasswordHash, password);

			if (passwordResult != PasswordVerificationResult.Success)
			{
				return null; // Invalid password
			}

			// Return the user if the credentials are valid
			return user;
		}



		public string GenerateJwtToken(User user)
		{
			var claims = new[]
			{
		new Claim(ClaimTypes.Name, user.Username),
		new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) // Include UserId
    };

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer: _config["Jwt:Issuer"],
				audience: _config["Jwt:Audience"],
				claims: claims,
				expires: DateTime.UtcNow.AddHours(1),
				signingCredentials: creds);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		public async Task<User?> ValidateUserAsync(string username, string password)
		{
			var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
			if (user == null) return null;

			var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, password);
			return result == PasswordVerificationResult.Success ? user : null;
		}

	}
}
