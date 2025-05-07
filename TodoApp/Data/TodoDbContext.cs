using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using TodoApp.Models;

namespace TodoApp.Data
{
	public class TodoDbContext : DbContext
	{
		public TodoDbContext(DbContextOptions<TodoDbContext> options)
			: base(options) { }

		public DbSet<Todo> Todos { get; set; }
	}
}
