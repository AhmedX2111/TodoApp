using System.ComponentModel.DataAnnotations;

namespace TodoApp.Models
{
	public class User
	{
		[Key]
		public Guid Id { get; set; } = Guid.NewGuid();

		[Required]
		[MaxLength(50)]
		public string Username { get; set; }

		[Required]
		public string PasswordHash { get; set; }
	}
}
