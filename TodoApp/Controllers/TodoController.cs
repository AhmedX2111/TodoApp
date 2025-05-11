using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using TodoApp.Data;
using TodoApp.Models;

namespace TodoApp.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class TodosController : ControllerBase
	{
		private readonly AppDbContext _context;
		private readonly ILogger<TodosController> _logger;

		public TodosController(AppDbContext context, ILogger<TodosController> logger)
		{
			_context = context;
			_logger = logger;
		}

		// GET: /api/todos?status=InProgress
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Todo>>> GetTodos([FromQuery] TodoStatus? status)
		{
			var userId = GetCurrentUserId();
			if (userId == null) return Unauthorized();

			var todos = await _context.Todos
				.Where(t => t.UserId == userId && (!status.HasValue || t.Status == status))
				.OrderByDescending(t => t.CreatedDate)
				.ToListAsync();

			_logger.LogInformation($"Retrieved {todos.Count} todos for user {userId}");
			return Ok(todos);
		}

		// POST: /api/todos
		[HttpPost]
		public async Task<ActionResult<Todo>> CreateTodo(Todo dto)
		{
			var userId = GetCurrentUserId();
			if (userId == null) return Unauthorized();

			var todo = new Todo
			{
				Id = Guid.NewGuid(),
				Title = dto.Title,
				Description = dto.Description,
				Status = dto.Status,
				Priority = dto.Priority,
				CreatedDate = DateTime.UtcNow,
				LastModifiedDate = DateTime.UtcNow,
				DueDate = dto.DueDate,
				UserId = userId.Value
			};

			_context.Todos.Add(todo);
			await _context.SaveChangesAsync();

			_logger.LogInformation($"Created new todo with ID {todo.Id} for user {userId}");
			return CreatedAtAction(nameof(GetTodoById), new { id = todo.Id }, todo);
		}

		// GET: /api/todos/{id}
		[HttpGet("{id}")]
		public async Task<ActionResult<Todo>> GetTodoById(Guid id)
		{
			var userId = GetCurrentUserId();
			_logger.LogInformation($"Requested todo: {id}, user: {userId}");

			if (userId == null) return Unauthorized();

			// First check if the todo exists at all
			var todoExists = await _context.Todos.AnyAsync(t => t.Id == id);
			if (!todoExists)
			{
				_logger.LogWarning($"Todo with ID {id} does not exist in the database");
				return NotFound($"Todo with ID {id} does not exist");
			}

			// Then check if it belongs to this user
			var todo = await _context.Todos
				.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

			if (todo == null)
			{
				_logger.LogWarning($"Todo with ID {id} exists but does not belong to user {userId}");
				return NotFound($"Todo not found for the current user");
			}

			_logger.LogInformation($"Successfully retrieved todo {id} for user {userId}");
			return Ok(todo);
		}

		// Add a diagnostic endpoint for testing
		[HttpGet("diagnostic")]
		[AllowAnonymous]
		public async Task<IActionResult> RunDiagnostic()
		{
			var result = new Dictionary<string, object>();

			// Count all todos
			var totalTodos = await _context.Todos.CountAsync();
			result.Add("TotalTodos", totalTodos);

			// Count all users
			var totalUsers = await _context.Users.CountAsync();
			result.Add("TotalUsers", totalUsers);

			// Get a sample of todos (limit to 5)
			var sampleTodos = await _context.Todos
				.Select(t => new { t.Id, t.Title, t.UserId })
				.Take(5)
				.ToListAsync();
			result.Add("SampleTodos", sampleTodos);

			return Ok(result);
		}

		// PUT: /api/todos/{id}
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateTodo(Guid id, [FromBody] Todo updated)
		{
			var userId = GetCurrentUserId();
			if (userId == null) return Unauthorized();

			if (id != updated.Id) return BadRequest("ID mismatch");
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var todo = await _context.Todos.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
			if (todo == null) return NotFound();

			todo.Title = updated.Title;
			todo.Description = updated.Description;
			todo.Status = updated.Status;
			todo.Priority = updated.Priority;
			todo.DueDate = updated.DueDate;
			todo.LastModifiedDate = DateTime.UtcNow;

			await _context.SaveChangesAsync();
			_logger.LogInformation($"Updated todo {id} for user {userId}");
			return NoContent();
		}

		// DELETE: /api/todos/{id}
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteTodo(Guid id)
		{
			var userId = GetCurrentUserId();
			if (userId == null) return Unauthorized();

			var todo = await _context.Todos.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
			if (todo == null) return NotFound();

			_context.Todos.Remove(todo);
			await _context.SaveChangesAsync();
			_logger.LogInformation($"Deleted todo {id} for user {userId}");
			return NoContent();
		}

		// PATCH: /api/todos/{id}/complete
		[HttpPatch("{id}/complete")]
		public async Task<IActionResult> MarkComplete(Guid id)
		{
			var userId = GetCurrentUserId();
			if (userId == null) return Unauthorized();

			var todo = await _context.Todos.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
			if (todo == null) return NotFound();

			todo.Status = TodoStatus.Completed;
			todo.LastModifiedDate = DateTime.UtcNow;

			await _context.SaveChangesAsync();
			_logger.LogInformation($"Marked todo {id} as complete for user {userId}");
			return NoContent();
		}

		private Guid? GetCurrentUserId()
		{
			var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (userIdClaim == null) return null;
			return Guid.TryParse(userIdClaim, out var userId) ? userId : (Guid?)null;
		}
	}
}