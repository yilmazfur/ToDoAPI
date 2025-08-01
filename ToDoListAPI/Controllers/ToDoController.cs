using Microsoft.AspNetCore.Mvc;
using MediatR;
using ToDoListAPI.Commands;
using ToDoListAPI.Queries;
using ToDoListAPI.Services;

namespace ToDoListAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ToDoController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly OpenAIService _openAIService;

        public ToDoController(IMediator mediator, OpenAIService openAIService)
        {
            _mediator = mediator;
            _openAIService = openAIService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ToDo>>> GetAll()
        {
            var todos = await _mediator.Send(new GetAllTodosQuery());
            return Ok(todos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ToDo>> Get(int id)
        {
            var todo = await _mediator.Send(new GetTodoByIdQuery(id));
            if (todo == null) return NotFound();
            return Ok(todo);
        }

        [HttpPost]
        public async Task<ActionResult<ToDo>> Create(CreateTodoCommand command)
        {
            var todo = await _mediator.Send(command);
            return CreatedAtAction(nameof(Get), new { id = todo.Id }, todo);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateTodoCommand command)
        {
            if (id != command.Id) return BadRequest("Id mismatch");

            var todo = await _mediator.Send(command);
            if (todo == null) return NotFound();
            
            return Ok(todo);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _mediator.Send(new DeleteTodoCommand(id));
            if (!success) return NotFound();
            
            return NoContent();
        }

        [HttpPost("OpenAI")]
        public async Task<IActionResult> GetGPTSuggestion([FromBody] string text)
        {
            var response = await _openAIService.GetResponseFromAI(text);
            if (response == null) return BadRequest("Could not parse task.");
            return Ok(response);
        }

        [HttpPost("OpenAI/suggest-tasks")]
        public async Task<ActionResult<TaskSuggestions>> GetTaskSuggestion([FromBody] string text)
        {
            var response = await _openAIService.GetTaskSuggestions(text);
            if (response == null) return BadRequest("Could not parse task.");
            return Ok(response);
        }
    }
}
