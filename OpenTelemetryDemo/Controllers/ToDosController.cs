using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenTelemetryDemo.Data;

namespace OpenTelemetryDemo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ToDosController(IDbContextFactory<ApplicationDbContext> dbFactory) : Controller
{
	private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory;

	[HttpGet]
	public async Task<IActionResult> GetAll()
	{
		await using var db = _dbFactory.CreateDbContext();
		var todos = await db.ToDos.ToListAsync();
		return Ok(todos);
	}

	[HttpGet("{id}", Name = "GetById")]
	public async Task<IActionResult> GetById(int id)
	{
		await using var db = _dbFactory.CreateDbContext();
		var todo = await db.ToDos.FindAsync(id);
		if (todo == null) return NotFound();
		return Ok(todo);
	}

	[HttpPost]
	public async Task<IActionResult> Insert(ToDo todo)
	{
		await using var db = _dbFactory.CreateDbContext();
		db.ToDos.Add(todo);
		await db.SaveChangesAsync();
		return Ok(new { todo.Id });

		// can't figure out how to get this to work
		//return CreatedAtAction(nameof(GetById), todo.Id);
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> Update(int id, ToDo todo)
	{
		await using var db = _dbFactory.CreateDbContext();
		var existing = await db.ToDos.FindAsync(id);
		if (existing == null)
		{
			return NotFound();
		}
		existing.Description = todo.Description;
		existing.IsComplete = todo.IsComplete;
		await db.SaveChangesAsync();
		return Ok();
	}
}
