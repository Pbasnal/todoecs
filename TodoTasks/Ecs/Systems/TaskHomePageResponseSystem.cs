namespace TodoApp
{
    public class TaskHomePageResponseSystem : ISystem<TaskEntity>
    {
        private readonly ILogger _logger;

        public TaskHomePageResponseSystem()
        {
            // _logger = logger;
        }

        public void Execute(EntityArchetype<TaskEntity> entityArchetype)
        {
            var responsePool = entityArchetype.GetComponentPool<TaskQuickViewResponseComponent>();
            var quickViewTitlesPool = entityArchetype.GetComponentPool<TaskQuickViewTitles>();

            foreach (var entity in entityArchetype.GetActiveEntities())
            {
                if (entity == null)
                {
                    Console.WriteLine("entity is null");
                }

                ref var quickViewTitles = ref quickViewTitlesPool.GetElementAt(entity.Index);
                ref var response = ref responsePool.GetElementAt(entity.Index);

                if (quickViewTitles.taskTitle == null)
                {
                    continue;
                }
                
                response.taskQuickViewCards = new TaskQuickViewCard[quickViewTitles.taskTitle.Length];

                // _logger.LogDebug($"Number responses > {quickViewTitles.taskTitle.Length}");

                for (int titleIndex = 0; titleIndex < quickViewTitles.taskTitle.Length; titleIndex++)
                {
                    response.taskQuickViewCards[titleIndex].title = quickViewTitles.taskTitle[titleIndex];
                    // _logger.LogDebug($"Title {response.taskQuickViewCards[titleIndex].title}");
                }
            }
        }
    }
}