using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApp.Data;
using TodoApp.Models;

namespace TodoApp.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class TodoController : ControllerBase
	{
		private readonly TodoDbContext _context;

		public TodoController(TodoDbContext context)
		{
			_context = context;
		}

		// GET: api/todo?status=Pending
		[HttpGet]
		public async Task<IActionResult> GetTodos([FromQuery] TodoStatus? status)
		{
			var todos = await _context.Todos
				.Where(t => !status.HasValue || t.Status == status.Value)
				.ToListAsync();

			return Ok(todos);
		}

		// GET: api/todo/{id}
		[HttpGet("{id}")]
		public async Task<IActionResult> GetTodo(Guid id)
		{
			var todo = await _context.Todos.FindAsync(id);
			if (todo == null) return NotFound();
			return Ok(todo);
		}

		// POST: api/todo
		[HttpPost]
		public async Task<IActionResult> CreateTodo([FromBody] Todo todo)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);

			todo.Id = Guid.NewGuid();
			todo.CreatedDate = DateTime.UtcNow;

			_context.Todos.Add(todo);
			await _context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetTodo), new { id = todo.Id }, todo);
		}



		// PUT: api/todo/{id}
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateTodo(Guid id, Todo updatedTodo)
		{
			if (id != updatedTodo.Id) return BadRequest("ID mismatch");
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var todo = await _context.Todos.FindAsync(id);
			if (todo == null) return NotFound();

			todo.Title = updatedTodo.Title;
			todo.Description = updatedTodo.Description;
			todo.Status = updatedTodo.Status;
			todo.Priority = updatedTodo.Priority;
			todo.DueDate = updatedTodo.DueDate;
			todo.LastModifiedDate = DateTime.UtcNow;

			await _context.SaveChangesAsync();
			return NoContent();
		}

		// PATCH: api/todo/{id}/complete
		[HttpPatch("{id}/complete")]
		public async Task<IActionResult> MarkAsComplete(Guid id)
		{
			var todo = await _context.Todos.FindAsync(id);
			if (todo == null) return NotFound();

			todo.Status = TodoStatus.Completed;
			todo.LastModifiedDate = DateTime.UtcNow;

			await _context.SaveChangesAsync();
			return NoContent();
		}

		// DELETE: api/todo/{id}
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteTodo(Guid id)
		{
			var todo = await _context.Todos.FindAsync(id);
			if (todo == null) return NotFound();

			_context.Todos.Remove(todo);
			await _context.SaveChangesAsync();
			return NoContent();
		}
	}
}
