using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TodoApp.Models
{
	public enum TodoStatus { Pending , InProgress, Completed }
	public enum TodoPriority
	{
		Low ,
		Medium ,
		High
	}
	public class Todo
	{
		public Guid Id { get; set; }

		[Required, MaxLength(100)]
		public string Title { get; set; } = string.Empty;

		public string? Description { get; set; }

		[Required]
		public TodoStatus Status { get; set; } = TodoStatus.Pending;

		[Required]
		public TodoPriority Priority { get; set; } = TodoPriority.Medium;

		public DateTime? DueDate { get; set; }

		public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
		public DateTime LastModifiedDate { get; set; } = DateTime.UtcNow;

		// Foreign Key
		public Guid UserId { get; set; }
		[JsonIgnore]
		public User? User { get; set; } = null!;
	}
}
