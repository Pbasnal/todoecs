namespace ECSFramework.Ecs.System;

internal class EntityFinalizerSystem : ISystem
{
    public string Name => GetType().Name;

    public void Execute<INIT_COMPONENT, FINAL_COMPONENT>(AnEntityArchetype<INIT_COMPONENT, FINAL_COMPONENT> entityArchetype, int batchSize, CancellationToken token)
        where INIT_COMPONENT : struct, IComponent
        where FINAL_COMPONENT : struct, IComponent
    {
        var finalizerPool = entityArchetype.GetComponentPool<FINAL_COMPONENT>();
        var finalizeComponents = finalizerPool.GetActiveObjects();

        var finalComponentSpan = finalizeComponents.Span;

        for (int i = 0; i < finalComponentSpan.Length; i++)
        {
            ref var finalComponent = ref finalComponentSpan[i];
            if (finalComponent.IsSet)
            {
                ExecuteSystemBatch(entityArchetype, finalComponent);
            }
        }

        //var systemTasks = new List<Task>();
        //for (int i = 0; i < finalizeComponents.Length; i += batchSize)
        //{
        //    var len = finalizeComponents.Length > i + batchSize ? batchSize : finalizeComponents.Length - i;
        //    systemTasks.Add(StartSystem(entityArchetype, finalizeComponents.Slice(i, len)));
        //}

        //Task.WaitAll(systemTasks.ToArray(), token);
    }

    private async Task StartSystem<INIT_COMPONENT, FINAL_COMPONENT>(AnEntityArchetype<INIT_COMPONENT, FINAL_COMPONENT> entityArchetype,
        Memory<FINAL_COMPONENT> finalComponents)
        where INIT_COMPONENT : struct, IComponent
        where FINAL_COMPONENT : struct, IComponent
    {
        await Task.Factory.StartNew(() =>
        {
            var finalComponentSpan = finalComponents.Span;

            for (int i = 0; i < finalComponentSpan.Length; i++)
            {
                ref var finalComponent = ref finalComponentSpan[i];
                if (finalComponent.IsSet)
                {
                    ExecuteSystemBatch(entityArchetype, finalComponent);
                }
            }
        });
    }

    protected void ExecuteSystemBatch<INIT_COMPONENT, FINAL_COMPONENT>(AnEntityArchetype<INIT_COMPONENT, FINAL_COMPONENT> entityArchetype,
        FINAL_COMPONENT finalComponent)
        where INIT_COMPONENT : struct, IComponent
        where FINAL_COMPONENT : struct, IComponent
    {
        try
        {
            entityArchetype.FreeEntity(finalComponent.EntityId);
            finalComponent.IsSet = true;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

}
