using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TodoApp.Dto;
using TodoApp.Interfaces;
using TodoApp.Models;

namespace TodoApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoAppController : Controller
    {
        private readonly ITodoListRepository _todoListRepository;
        private readonly IMapper _mapper;
        public TodoAppController(ITodoListRepository todoListRepository, IMapper mapper)
        {
            _todoListRepository = todoListRepository;
            _mapper = mapper;
        }

        [HttpGet, Authorize(Roles = "Admin,User")] 
        [ProducesResponseType(200, Type = typeof(IEnumerable<Todo>))]
        public IActionResult GetTodos()
        {
            var todos = _todoListRepository.GetTodos();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!User.IsInRole("Admin"))
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                todos = todos.Where(todo => todo.CreatedByUserId == userId).ToList();
            }

            return Ok(todos);
        }

        [HttpGet("{todoId}"), Authorize(Roles = "Admin,User")]
        [ProducesResponseType(200, Type = typeof(Todo))]
        [ProducesResponseType(400)]
        public IActionResult GetTodo(int todoId)
        {
            if (!_todoListRepository.TodoExists(todoId))
                return NotFound();

            var todo = _todoListRepository.GetTodoList(todoId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!User.IsInRole("Admin") && todo.CreatedByUserId != User.FindFirst(ClaimTypes.NameIdentifier)?.Value)
            {
                return Forbid();
            }

            return Ok(todo);
        }

        [HttpPost, Authorize(Roles = "Admin,User")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]

        public IActionResult CreateTodo([FromBody] TodoListDto todoCreate)
        {
            if (todoCreate == null)
                return BadRequest(ModelState);

            if(User.Identity.IsAuthenticated)
            {

                var createdByUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var category = _todoListRepository.GetTodos().FirstOrDefault(t => t.Title.Trim().ToUpper().Equals(todoCreate.Title.TrimEnd().ToUpper()));

                if (category != null)
                {
                    ModelState.AddModelError("", "Category alrady exists");
                    return StatusCode(402, ModelState);
                }

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var todoMap = _mapper.Map<Todo>(todoCreate);
                todoMap.CreatedByUserId = createdByUserId;

                if (!_todoListRepository.CreateTodo(todoMap))
                {
                    ModelState.AddModelError("", "Something went wrong while saving");
                    return StatusCode(500, ModelState);
                }

                return Ok("Successfully created");


            }
            else
            {
                return Unauthorized();  
            }

            
        }

        [HttpPut("{todoId}"), Authorize(Roles = "Admin,User")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateTodo(int todoId, [FromBody] TodoListDto updatedTodo)
        {
            if (updatedTodo == null)
                return BadRequest(ModelState);

            if (todoId != updatedTodo.Id)
                return BadRequest(ModelState);

            if (!_todoListRepository.TodoExists(todoId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var todoToUpdate = _todoListRepository.GetTodoList(todoId);

            if (!User.IsInRole("Admin") && todoToUpdate.CreatedByUserId != User.FindFirst(ClaimTypes.NameIdentifier)?.Value)
            {
                return Forbid();
            }


            var todoMap = _mapper.Map<Todo>(updatedTodo);

            if (!_todoListRepository.UpdateTodo(todoMap))
            {
                ModelState.AddModelError("", "Something went wrong updating category");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{todoId}"), Authorize(Roles = "Admin,User")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteTodo(int todoId)
        {
            if (!_todoListRepository.TodoExists(todoId))
                return NotFound();

            var todoToDelete = _todoListRepository.GetTodoList(todoId);


            if (!User.IsInRole("Admin") && todoToDelete.CreatedByUserId != User.FindFirst(ClaimTypes.NameIdentifier)?.Value)
            {
                return Forbid();
            }


            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_todoListRepository.DeleteTodo(todoToDelete))
            {
                ModelState.AddModelError("", "Something went wrong deleting todo");
            }

            return NoContent();
        }
    }
}
