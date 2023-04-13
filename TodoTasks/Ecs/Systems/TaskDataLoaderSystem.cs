using Prometheus;
using todorest;

namespace TodoApp
{
    public class TaskDataLoaderSystem : ISystem<TaskEntity>
    {
        private DataStore dataStore;
        private readonly ILogger _logger;

        private static readonly Counter entityLoadCount = Metrics
            .CreateCounter("entity_loaded_count", "Number of entities loaded by data loader.");
        

        public TaskDataLoaderSystem()
        {
            // _logger = logger;
            dataStore = DataStore.GetDataStore(1000);
        }

        public void Execute(EntityArchetype<TaskEntity> entityArchetype)
        {
            using (TodoMetrics.MethodMetrics("TaskDataLoaderSystem"))
            {
                var requestPool = entityArchetype.GetComponentPool<TaskQuickViewRequestComponent>();
                var quickViewTitles = entityArchetype.GetComponentPool<TaskQuickViewTitles>();

                int numberOfEntitiesProcessed = 0;
                foreach (var entity in entityArchetype.GetActiveEntities())
                {
                    numberOfEntitiesProcessed++;
                    ref var request = ref requestPool.GetElementAt(entity.Index);
                    // Console.WriteLine($"Requested system for > {request.page} number of tasks: {request.numberOfTasks}");
                    var taskTitles = dataStore.GetTaskTitles(request.page, request.numberOfTasks);

                    string[] titles = new string[taskTitles.Count];
                    // Console.WriteLine($"Number of tasks loaded from data store > {taskTitles.Count}");

                    int titleIndex = 0;
                    foreach (var title in taskTitles)
                    {
                        titles[titleIndex++] = title.Value;
                    }

                    ref var quickViewTitle = ref quickViewTitles.GetElementAt(entity.Index);

                    quickViewTitle.taskTitle = titles;
                }
                entityLoadCount.Inc(numberOfEntitiesProcessed);
            }
        }
    }
}