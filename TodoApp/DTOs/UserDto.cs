namespace TodoApp.DTOs
{
	public class UserDto
	{
		public class RegisterDto
		{
			public string Username { get; set; }
			public string Password { get; set; }
		}

		public class LoginDto
		{
			public string Username { get; set; }
			public string Password { get; set; }
		}

	}
}
