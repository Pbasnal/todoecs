using Microsoft.AspNetCore.Mvc;
using TodoApp;

namespace todorest.Controllers;

[ApiController]
[Route("[controller]")]
public class TodoTasksOopController : ControllerBase
{
    private readonly ILogger<TodoTasksOopController> _logger;
    private TodoTasksService todoTasksService;

    public TodoTasksOopController(ILogger<TodoTasksOopController> logger)
    {
        _logger = logger;
        todoTasksService = new TodoTasksService(logger);
    }

    [HttpGet(Name = "getTasksOop")]
    public List<TodoTask> GetOopTasks()
    {
        int page = 0;
        int numberOfElements = 500;

        _logger.LogDebug($"Requesting OOP for > page: {page} count: {numberOfElements}");

        // using (TodoMetrics.MethodMetrics("OopTodoAPI"))
        {
            return todoTasksService.GetTasks(page, numberOfElements);
        }
    }
}
