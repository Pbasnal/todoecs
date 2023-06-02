using ECSFramework;

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
    public class TaskEntityArchetype : AnEntityArchetype<TaskEntity>
    {
        private ComponentPoolDod<TaskQuickViewRequestComponent> requestComponents;
        private ComponentPoolDod<TaskQuickViewTitles> titlesComponents;
        private ComponentPoolDod<TaskQuickViewResponseComponent> responseComponents;

        public TaskEntityArchetype(int initialNumberOfEntities) :
            base(initialNumberOfEntities)
        {
            requestComponents = new ComponentPoolDod<TaskQuickViewRequestComponent>();
            titlesComponents = new ComponentPoolDod<TaskQuickViewTitles>();
            responseComponents = new ComponentPoolDod<TaskQuickViewResponseComponent>();

            RegisterComponentPool(ComponentType.QUICK_VIEW_REQUEST, requestComponents);
            RegisterComponentPool(ComponentType.QUICK_VIEW_TASK_TITLES, titlesComponents);
            RegisterComponentPool(ComponentType.QUICK_VIEW_RESPONSE, responseComponents);
        }

        public TaskEntity BuildQuickViewEntity(int page, int numberOfTasks)
        {
            ref var entity = ref CreateEntity();

            ref var requestComponent = ref requestComponents.GetFreeObject();
            requestComponent.page = page;
            requestComponent.numberOfTasks = numberOfTasks;

            ref var titleComponent = ref titlesComponents.GetFreeObject();
            //viewComponent.taskTitle = new string[numberOfTasks];

            ref var responseComponent = ref responseComponents.GetFreeObject();
            responseComponent.taskQuickViewCards = new TaskQuickViewCard[numberOfTasks];

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