using System.ComponentModel.DataAnnotations;

namespace TodoApp.DTOs
{
	public class UserDto
	{
		public class RegisterDto
		{
			[Required]
			public string Username { get; set; }
			[Required]
			public string Password { get; set; }
		}

		public class LoginDto
		{
			[Required]
			public string Username { get; set; }
			[Required]
			public string Password { get; set; }
		}

	}
}
