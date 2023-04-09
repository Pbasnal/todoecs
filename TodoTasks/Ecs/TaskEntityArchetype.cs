using Prometheus;
using todorest;

namespace TodoApp
{
    public class TaskEntityArchetype : EntityArchetype<TaskEntity>
    {
        // private readonly ILogger _logger;
        private static readonly Counter archetypeCount = Metrics
                .CreateCounter("task_entity_archetype_count", "Number of todo task controllers created.");

        public TaskEntityArchetype()
        {
            archetypeCount.Inc();
            //    _logger = logger;
        }

        public TaskEntity BuildQuickViewEntity(int page, int numberOfTasks)
        {
            using (TodoMetrics.MethodMetrics("BuildQuickViewEntity"))
            {
                var entity = CreateEntity();

                ref var requestComponent = ref CreateComponentOfEntity<TaskQuickViewRequestComponent>(entity);
                requestComponent.page = page;
                requestComponent.numberOfTasks = numberOfTasks;

                CreateComponentOfEntity<TaskQuickViewTitles>(entity);
                CreateComponentOfEntity<TaskQuickViewResponseComponent>(entity);
                // AddComponentToEntity<TaskQuickViewData>(entity);
                // AddComponentToEntity<TaskQuickViewDates>(entity);
                // AddComponentToEntity<TaskQuickViewReminders>(entity);
                return entity;
            }
        }

        public List<ISystem<TaskEntity>> GetQuickSystems()
        {
            return new List<ISystem<TaskEntity>>
            {
                new TaskDataLoaderSystem(),
                new TaskHomePageResponseSystem()
            };
        }

        public void ExecuteSystems(List<ISystem<TaskEntity>> systems)
        {
            foreach (var system in systems)
            {
                using (TodoMetrics.MethodMetrics(system.GetType().Name))
                {
                    system.Execute(this);
                }
            }
        }
    }
}