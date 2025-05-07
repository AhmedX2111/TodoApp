using System.ComponentModel.DataAnnotations;

namespace TodoApp.Models
{
	public enum TodoStatus { Pending = 0, InProgress = 1, Completed = 2 }
	public enum TodoPriority
	{
		Low = 0,
		Medium = 1,
		High = 2
	}
	public class Todo
	{
		public Guid Id { get; set; }

		[Required]
		public string Title { get; set; }

		public string Description { get; set; }

		[Required]
		public DateTime DueDate { get; set; }

		[Required]
		public TodoStatus Status { get; set; }

		[Required]
		public int Priority { get; set; }

		public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

		public DateTime? LastModifiedDate { get; set; }
	}
}
