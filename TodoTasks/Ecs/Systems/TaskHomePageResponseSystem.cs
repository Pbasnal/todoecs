using ECSFramework;

namespace TodoApp;

public class TaskHomePageResponseSystem : ISystem<TaskEntity>
{
    public string Name => "TaskHomePageResponseSystem";

    public void Execute(AnEntityArchetype<TaskEntity> entityArchetype)
    {
        var responsePool = (ComponentPoolDod<TaskQuickViewResponseComponent>)entityArchetype
            .GetComponentPool(ComponentType.QUICK_VIEW_RESPONSE);

        var quickViewTitlesPool = (ComponentPoolDod<TaskQuickViewTitles>)entityArchetype
            .GetComponentPool(ComponentType.QUICK_VIEW_TASK_TITLES);

        var titlesSpan = quickViewTitlesPool.GetSpanOfActiveObjects();
        var responseSpan = responsePool.GetSpanOfActiveObjects();

        var rangeOfIteration = Math.Min(titlesSpan.Length, responseSpan.Length);

        for (int i = 0; i < rangeOfIteration; i++)
        {
            ref var title = ref titlesSpan[i];
            ref var response = ref responseSpan[i];

            if (response.IsSet
                || response.taskQuickViewCards == null
                || title.taskTitle.IsEmpty)
                continue;

            var viewResponseTitlesSpan = title.taskTitle.Span;
            for (int titleIndex = 0; titleIndex < response.taskQuickViewCards.Length; titleIndex++)
            {
                response.taskQuickViewCards[titleIndex].title = viewResponseTitlesSpan[titleIndex];
            }
            response.IsSet = true;
        }
    }

    public void Execute(AnEntityArchetype<TaskEntity> entityArchetype, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public void ExecuteBatch(AnEntityArchetype<TaskEntity> entityArchetype, int start, int end, CancellationToken token)
    {
        throw new NotImplementedException();
    }
}