using Microsoft.AspNetCore.Mvc;
using Prometheus;
using TodoApp;

namespace todorest.Controllers;

[ApiController]
[Route("[controller]")]
public class TodoTasksController : ControllerBase
{
    private readonly ILogger<TodoTasksController> _logger;
    private readonly static TaskEntityArchetype _taskEntityArchetype
        = new TaskEntityArchetype();


    public TodoTasksController(ILogger<TodoTasksController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "getTasksEcs")]
    public TaskQuickViewResponseComponent GetEcsTasks()
    {
        int page = 0;
        int numberOfElements = 500;

        _logger.LogDebug($"Requesting for > page: {page} count: {numberOfElements}");

        using (TodoMetrics.MethodMetrics("EcsTodoAPI"))
        {
            var quickViewEntity = _taskEntityArchetype.BuildQuickViewEntity(page, numberOfElements);
            var systemsToExecute = _taskEntityArchetype.GetQuickSystems();
            _taskEntityArchetype.ExecuteSystems(systemsToExecute);

            return _taskEntityArchetype.GetResponse<TaskQuickViewResponseComponent>(quickViewEntity);
        }
    }
}
