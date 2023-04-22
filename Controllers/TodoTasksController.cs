using Microsoft.AspNetCore.Mvc;
using Prometheus;
using TodoApp;

namespace todorest.Controllers;


/*
* * In the Data Oriented Design (DOD) mindset, if we are doing anything once, then we probably have to do it multiple times.
* * Based on this philosophy we want to put multiple requests in a single array and process them iteratively.

! But any normal web server processes each request in a separate thread/task/future etc.
? So to solve, this we need to capture all requests and batch them.
* * In this project, we are creating one static object of type *TaskEntityArchetype* which will hold all
* * entities and components. And since dotnet creates a new task for each request, the static variable will be shared
* * among all the requests since task share the same memory.

Input of 10000 is the initial number of entities that we are expecting to have at any given point of time. If 
we get more than that, then EntityArchetype will create new entities entities.

* * The code uses Entity Component System (ECS) architecture to implement DOD. This is not a pure DOD style of programming
* * But it's very close. Additionaly ECS it self provides a lot of benefits to the code architecture that can be useful 
* * without DOD. Those benefits will be covered in a separate repo.
*/

[ApiController]
[Route("[controller]")]
public class TodoTasksController : ControllerBase
{
    private readonly ILogger<TodoTasksController> _logger;

    private readonly static TaskEntityArchetype _taskEntityArchetype
        = new TaskEntityArchetype(10000);


    public TodoTasksController(ILogger<TodoTasksController> logger)
    {
        _logger = logger;
    }


    /*
    The flow of this method is to create an entity that represents the operation that's going to happen in this api.
    Here we are trying to get a list of Tasks from the DataStore to show them to the user. Hence the Entity name is
    QuickViewEntity. 
    ! One important callout, in ECS every entity is nothing but an ID, an integer. It's like aadhar card number. Just 
    ! a number.

    Every api can follow the same flow - 
    1. Create an entity which defines the request. 
    2. Get the systems that should get executed on the entity.
    3. Execute the systems.
    4. Get the response component of the entity.

    ! Possible improvement
    We are executing all the systems everytime in this code. But when a system is executing, it processes all active entities.
    We can optimise the system by skipping the entities which have been processed already per system.
    */
    [HttpGet(Name = "getTasksEcs")]
    public TaskQuickViewResponseComponent GetEcsTasks()
    {
        int page = 0;
        int numberOfElements = 500;

        _logger.LogDebug($"Requesting for > page: {page} count: {numberOfElements}");
        
        var quickViewEntity = _taskEntityArchetype.BuildQuickViewEntity(page, numberOfElements);
        var systemsToExecute = _taskEntityArchetype.GetQuickSystems();
        _taskEntityArchetype.ExecuteSystems(systemsToExecute);

        return _taskEntityArchetype.GetResponse<TaskQuickViewResponseComponent>(quickViewEntity);
    }
}
