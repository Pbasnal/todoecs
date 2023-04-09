namespace TodoApp
{
    public class TaskDataLoaderSystem : ISystem<TaskEntity>
    {
        private DataStore dataStore;
        private readonly ILogger _logger;

        public TaskDataLoaderSystem()
        {
            // _logger = logger;
            dataStore = DataStore.GetDataStore(1000);
        }

        public void Execute(EntityArchetype<TaskEntity> entityArchetype)
        {
            var requestPool = entityArchetype.GetComponentPool<TaskQuickViewRequestComponent>();
            var quickViewTitles = entityArchetype.GetComponentPool<TaskQuickViewTitles>();

            foreach (var entity in entityArchetype.GetActiveEntities())
            {
                ref var request = ref requestPool.GetElementAt(entity.Index);
                // _logger.LogDebug($"Requested system for > {request.page} number of tasks: {request.numberOfTasks}");
                var taskTitles = dataStore.GetTaskTitles(request.page, request.numberOfTasks);

                string[] titles = new string[taskTitles.Count];
                // _logger.LogDebug($"Number of tasks loaded from data store > {taskTitles.Count}");

                int titleIndex = 0;
                foreach (var title in taskTitles)
                {
                    titles[titleIndex++] = title.Value;
                }

                ref var quickViewTitle = ref quickViewTitles.GetElementAt(entity.Index);

                quickViewTitle.taskTitle = titles;
            }
        }
    }
}