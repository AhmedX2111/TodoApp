using System.ComponentModel.DataAnnotations;

namespace TodoApp.Models
{
	public class User
	{
		public Guid Id { get; set; } // pk

		[Required]
		[MaxLength(50)]
		public string Username { get; set; } = string.Empty;

		[Required]
		public string PasswordHash { get; set; } = string.Empty;

		// Navigation
		public ICollection<Todo> Todos { get; set; } = new List<Todo>();
	}
}
