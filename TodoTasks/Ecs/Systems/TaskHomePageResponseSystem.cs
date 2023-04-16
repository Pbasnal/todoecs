namespace TodoApp
{
    public class TaskHomePageResponseSystem : ISystem<TaskEntity>
    {
        public void Execute(EntityArchetype<TaskEntity> entityArchetype)
        {
            var responsePool = entityArchetype.GetComponentPool<TaskQuickViewResponseComponent>();
            var quickViewTitlesPool = entityArchetype.GetComponentPool<TaskQuickViewTitles>();

            foreach (var entity in entityArchetype.GetActiveEntities())
            {
                ref var response = ref responsePool.GetElementAt(entity.Index);
                if (response.IsSet) continue;

                ref var quickViewTitles = ref quickViewTitlesPool.GetElementAt(entity.Index);
                if (quickViewTitles.taskTitle == null) continue;
                
                response.taskQuickViewCards = new TaskQuickViewCard[quickViewTitles.taskTitle.Length];
                for (int titleIndex = 0; titleIndex < quickViewTitles.taskTitle.Length; titleIndex++)
                {
                    response.taskQuickViewCards[titleIndex].title = quickViewTitles.taskTitle[titleIndex];
                }

                response.IsSet = true;
            }
        }
    }
}