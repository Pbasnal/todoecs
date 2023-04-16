namespace TodoApp
{
    /*
    * * TaskEntityArchetype gives us a clean way to perform api specific operations i.e. creating 
    * * request component with proper data. 
    * * It also knows what components are needed for a given api and what systems should be executed
    * * and the order of execution.
    * *
    * * Suffice to say that this is where we connect our lego pieces and build a feature.
    */
    public class TaskEntityArchetype : EntityArchetype<TaskEntity>
    {
        public TaskEntityArchetype(int initialNumberOfEntities) :
            base(initialNumberOfEntities)
        {}

        public TaskEntity BuildQuickViewEntity(int page, int numberOfTasks)
        {
            var entity = CreateEntity();
            ref var requestComponent = ref CreateComponentOfEntity<TaskQuickViewRequestComponent>(entity);
            requestComponent.page = page;
            requestComponent.numberOfTasks = numberOfTasks;

            CreateComponentOfEntity<TaskQuickViewTitles>(entity);
            CreateComponentOfEntity<TaskQuickViewResponseComponent>(entity);
            return entity;
        }

        public List<ISystem<TaskEntity>> GetQuickSystems()
        {
            return new List<ISystem<TaskEntity>>
            {
                new TaskDataLoaderSystem(),
                new TaskHomePageResponseSystem()
            };
        }

        /*
        * Currently we are executing all systems sequentially since they have dependency on each
        * other but this is where we can control parallelism of systems when we don't have 
        * interdependent systems.
        */
        public void ExecuteSystems(List<ISystem<TaskEntity>> systems)
        {
            foreach (var system in systems)
            {
                system.Execute(this);
            }
        }
    }
}