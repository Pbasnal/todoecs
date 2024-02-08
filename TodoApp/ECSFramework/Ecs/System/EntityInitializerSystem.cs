namespace ECSFramework.Ecs.System;

internal class EntityInitializerSystem : ISystem
{
    public string Name => GetType().Name;

    public void Execute<INIT_COMPONENT, FINAL_COMPONENT>(AnEntityArchetype<INIT_COMPONENT, FINAL_COMPONENT> entityArchetype,
        int batchSize, CancellationToken token)
        where INIT_COMPONENT : struct, IComponent
        where FINAL_COMPONENT : struct, IComponent
    {
        var initializerPool = entityArchetype.GetComponentPool<INIT_COMPONENT>();
        var initializerComponents = initializerPool.GetActiveObjects();
        
        var entitySpan = entityArchetype.GetActiveEntities().Span;
        var initComponentSpan = initializerComponents.Span;

        //var systemTasks = new List<Task>();
        for (int i = 0; i < initComponentSpan.Length; i++)
        {
            ref var initComponent = ref initComponentSpan[i];
            if (initComponent.IsSet) continue;

            ref var entity = ref entitySpan[initComponent.EntityId];

            entityArchetype.AddComponentsToEntity(ref entity, ref initComponent);

            initComponent.IsSet = true;
        }
        //for (int i = 0; i < initializerComponents.Length; i += batchSize)
        //{
        //    var len = initializerComponents.Length > i + batchSize ? batchSize : initializerComponents.Length - i;
        //    systemTasks.Add(StartSystem(entityArchetype, initializerComponents.Slice(i, len)));
        //}

        //Task.WaitAll(systemTasks.ToArray(), token);
    }

    private async Task StartSystem<INIT_COMPONENT, FINAL_COMPONENT>(AnEntityArchetype<INIT_COMPONENT, FINAL_COMPONENT> entityArchetype,
        Memory<INIT_COMPONENT> initComponents)
        where INIT_COMPONENT : struct, IComponent
        where FINAL_COMPONENT : struct, IComponent
    {
        await Task.Factory.StartNew(() => ExecuteSystemBatch(entityArchetype, initComponents));
    }

    protected void ExecuteSystemBatch<INIT_COMPONENT, FINAL_COMPONENT>(AnEntityArchetype<INIT_COMPONENT, FINAL_COMPONENT> entityArchetype,
        Memory<INIT_COMPONENT> initComponents)
        where INIT_COMPONENT : struct, IComponent
        where FINAL_COMPONENT : struct, IComponent
    {
        var entitySpan = entityArchetype.GetActiveEntities().Span;
        var initComponentSpan = initComponents.Span;

        for (int i = 0; i < initComponentSpan.Length; i++)
        {
            ref var initComponent = ref initComponentSpan[i];
            if (initComponent.IsSet) continue;

            ref var entity = ref entitySpan[initComponent.EntityId];

            entityArchetype.AddComponentsToEntity(ref entity, ref initComponent);

            initComponent.IsSet = true;
        }
    }
}
