using TodoApp;

namespace todorest
{
    public class TodoTasksService
    {
        private DataStore dataStore;
        private readonly ILogger _logger;

        public TodoTasksService(ILogger logger)
        {
            _logger = logger;
            dataStore = DataStore.GetDataStore(1000);
        }

        public List<TodoTask> GetTasks(int page, int count)
        {
            var tasks = new List<TodoTask>();

            Memory<string> taskTitles;
            using (TodoMetrics.MethodMetrics("OopDataRead"))
            {
                taskTitles = dataStore.GetTaskTitles(page, count);
            }

            using (TodoMetrics.MethodMetrics("OopBuildingResponse"))
            {
                foreach (var taskTitle in taskTitles.Span)
                {
                    tasks.Add(new TodoTask
                    {
                        taskQuickViewOopCard = new TaskQuickViewOopCard
                        {
                            title = taskTitle
                        }
                    });
                }
            }

            return tasks;
        }
    }
}