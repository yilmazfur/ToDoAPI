using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoListAPI;
using ToDoListAPI.Services;

namespace ToDoListAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ToDoController : ControllerBase
    {
        private readonly ToDoContext _context;
        private readonly OpenAIService _openAIService;

        public ToDoController(ToDoContext context, OpenAIService openAIService)
        {
            _context = context;
            _openAIService = openAIService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ToDo>>> GetAll()
        {
            return Ok(await _context.ToDos.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ToDo>> Get(int id)
        {
            var todo = await _context.ToDos.FindAsync(id);
            if (todo == null) return NotFound();
            return Ok(todo);
        }

        [HttpPost]
        public async Task<ActionResult<ToDo>> Create(ToDo todo)
        {
            _context.ToDos.Add(todo);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = todo.Id }, todo);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ToDo updated)
        {
            var todo = await _context.ToDos.FindAsync(id);
            if (todo == null) return NotFound();
            todo.TaskName = updated.TaskName;
            todo.IsCompleted = updated.IsCompleted;
            todo.Deadline = updated.Deadline;
            await _context.SaveChangesAsync();
            return Ok(todo);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var todo = await _context.ToDos.FindAsync(id);
            if (todo == null) return NotFound();
            _context.ToDos.Remove(todo);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("OpenAI")]
        public async Task<IActionResult> GetGPTSuggestion([FromBody] string text)
        {
            var response2 = await _openAIService.GetResponseFromAI(text);
            if (response2 == null) return BadRequest("Could not parse task.");
            return Ok(response2);
        }

        [HttpPost("OpenAI/suggest-tasks")]
        public async Task<ActionResult<TaskSuggestions>> GetTaskSuggestion([FromBody] string text)
        {
            var response2 = await _openAIService.GetTaskSuggestions(text);
            if (response2 == null) return BadRequest("Could not parse task.");
            return Ok(response2);
        }        
    }
}
