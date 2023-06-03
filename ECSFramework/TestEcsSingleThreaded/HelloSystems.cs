namespace ECSFramework;

public class MessageSetterSystem : ISystem<EcsEntity>
{
    public string Name => "MessageSetterSystem";

    public void Execute(AnEntityArchetype<EcsEntity> entityArchetype, CancellationToken token)
    {
        var componentPool = entityArchetype.GetComponentPool<HelloWorldMessageComponent>(0);
        var span = componentPool.GetActiveObjects();
        StartSystem(span).Wait();
    }

    public void ExecuteBatch(AnEntityArchetype<EcsEntity> entityArchetype, int batchSize, CancellationToken token)
    {
        var componentPool = entityArchetype.GetComponentPool<HelloWorldMessageComponent>(0);
        var memory = componentPool.GetActiveObjects();
        if (memory.IsEmpty) return;

        var systemTasks = new List<Task>();
        for (int i = 0; i < memory.Length; i += batchSize)
        {
            systemTasks.Add(StartSystem(memory.Slice(i, batchSize)));
        }

        Task.WaitAll(systemTasks.ToArray());
    }

    private async Task StartSystem(Memory<HelloWorldMessageComponent> memory)
    {
        await Task.Factory.StartNew(() =>
        {
            var span = memory.Span;
            for (int i = 0; i < span.Length; i++)
            {
                ref var component = ref span[i];
                if (component.IsSet) continue; // this is being set by the second system which is an anti pattern.
                                               // Future work to come up with an existance based approach
                component.helloWorldMessage.Set("Hello world " + component.Id);
            }
        });
    }
}

public class MessageConsolePrintSystem : ISystem<EcsEntity>
{
    public string Name => "MessageConsolePrintSystem";

    public void Execute(AnEntityArchetype<EcsEntity> entityArchetype, CancellationToken token)
    {
        var componentPool = entityArchetype
            .GetComponentPool<HelloWorldMessageComponent>(ComponentType.HELLO_WORLD_COMPONENT);

        var span = componentPool.GetActiveObjects();
        StartSystem(span).Wait();
    }

    public void ExecuteBatch(AnEntityArchetype<EcsEntity> entityArchetype, int batchSize, CancellationToken token)
    {
        var componentPool = entityArchetype
            .GetComponentPool<HelloWorldMessageComponent>(ComponentType.HELLO_WORLD_COMPONENT);

        var span = componentPool.GetActiveObjects();

        var systemTasks = new List<Task>();
        for (int i = 0; i < span.Length; i += batchSize)
        {
            systemTasks.Add(StartSystem(span.Slice(i, batchSize)));
        }

        Task.WaitAll(systemTasks.ToArray());
    }

    private async Task StartSystem(Memory<HelloWorldMessageComponent> memory)
    {
        await Task.Factory.StartNew(() =>
        {
            var span = memory.Span;
            for (int i = 0; i < span.Length; i++)
            {
                ref var component = ref span[i];

                if (component.IsSet) continue;

                Console.WriteLine(component.helloWorldMessage);
                component.IsSet = true;
            }
        });
    }
}

public class FinaliseEntitySystem : ISystem<EcsEntity>
{
    public string Name => "FinaliseEntitySystem";

    public void Execute(AnEntityArchetype<EcsEntity> entityArchetype, CancellationToken token)
    {
        var componentPool = entityArchetype
            .GetComponentPool<HelloWorldMessageComponent>(ComponentType.HELLO_WORLD_COMPONENT);
        var span = entityArchetype.GetActiveEntities();

        StartSystem(entityArchetype, span, componentPool).Wait();
    }

    public void ExecuteBatch(AnEntityArchetype<EcsEntity> entityArchetype, int batchSize, CancellationToken token)
    {
        var componentPool = entityArchetype
             .GetComponentPool<HelloWorldMessageComponent>(ComponentType.HELLO_WORLD_COMPONENT);
        var span = entityArchetype.GetActiveEntities();

        if (span.IsEmpty) return;

        var systemTasks = new List<Task>();
        for (int i = 0; i < span.Length; i += batchSize)
        {
            systemTasks.Add(StartSystem(entityArchetype,
                span.Slice(i, batchSize),
                componentPool));
        }
        Task.WaitAll(systemTasks.ToArray());
    }

    private async Task StartSystem(AnEntityArchetype<EcsEntity> entityArchetype, 
        Memory<EcsEntity> memory, 
        ComponentPoolDod<HelloWorldMessageComponent> componentPool)
    {
        await Task.Factory.StartNew((Action)(() =>
        {
            var span = memory.Span;
            for (int i = 0; i < span.Length; i++)
            {
                ref var entity = ref span[i];
                var componentId = entity.GetComponentId(ComponentType.HELLO_WORLD_COMPONENT);

                if (componentId == -1) continue;

                ref var component = ref componentPool.GetObjectWithId(componentId);

                if (component.IsSet)
                {
                    //Console.WriteLine($"\t system: {Name} entity: {entity.Id}");
                    entityArchetype.FreeEntity(entity.Id);
                }
            }
        }));
    }
}

