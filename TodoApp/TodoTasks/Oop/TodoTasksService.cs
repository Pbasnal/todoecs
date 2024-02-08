using TodoApp;

namespace todorest
{
    public class TodoTasksService
    {
        private DataStoreOop dataStore;
        private readonly ILogger _logger;

        public TodoTasksService(ILogger logger)
        {
            _logger = logger;
            dataStore = DataStoreOop.GetDataStore(1000);
        }

        public List<TodoTask> GetTasks(int page, int count)
        {
            var tasks = new List<TodoTask>();

            HashSet<TaskData> tasksData;
            using (TodoMetrics.MethodMetrics("OopDataRead"))
            {
                tasksData = dataStore.GetTaskTitles(page, count);
            }

            using (TodoMetrics.MethodMetrics("OopBuildingResponse"))
            {
                foreach (var taskData in tasksData)
                {
                    tasks.Add(new TodoTask
                    {
                        taskQuickViewOopCard = new TaskQuickViewOopCard
                        {
                            title = taskData.title
                        }
                    });
                }
            }

            return tasks;
        }
    }
}