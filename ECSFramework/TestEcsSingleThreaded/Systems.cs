namespace ECSFramework;

public class MessageSetterSystem : ISystem<EcsEntity>
{
    public string Name => "MessageSetterSystem";

    public void Execute(AnEntityArchetype<EcsEntity> entityArchetype, CancellationToken token)
    {
        var componentPool = entityArchetype.GetComponentPool<ComponentPoolDod<HelloWorldMessageComponent>>(0);

        var span = componentPool.GetSpanOfActiveObjects();

        for (int i = 0; i < span.Length; i++)
        {
            ref var component = ref span[i];
            if (component.IsSet) continue; // this is being set by the second system which is an anti pattern.
            // Future work to come up with an existance based approach
            component.helloWorldMessage.Set("Hello world " + component.Id);
        }
    }

    public void ExecuteBatch(AnEntityArchetype<EcsEntity> entityArchetype, int start, int end, CancellationToken token)
    {
        var componentPool = entityArchetype.GetComponentPool<ComponentPoolDod<HelloWorldMessageComponent>>(0);

        var span = componentPool.GetSpanOfActiveObjects();

        if(span.IsEmpty) return;

        for (int i = start; i < span.Length && i < end; i++)
        {
            ref var component = ref span[i];
            if (component.IsSet) continue; // this is being set by the second system which is an anti pattern.
            // Future work to come up with an existance based approach
            component.helloWorldMessage.Set("Hello world " + component.Id);
        }
    }
}

public class MessageConsolePrintSystem : ISystem<EcsEntity>
{
    public string Name => "MessageConsolePrintSystem";

    public void Execute(AnEntityArchetype<EcsEntity> entityArchetype, CancellationToken token)
    {
        var componentPool = entityArchetype
            .GetComponentPool<ComponentPoolDod<HelloWorldMessageComponent>>(ComponentType.HELLO_WORLD_COMPONENT);

        var span = componentPool.GetSpanOfActiveObjects();

        for (int i = 0; i < span.Length; i++)
        {
            ref var component = ref span[i];

            if (component.IsSet) continue;

            Console.WriteLine(component.helloWorldMessage);
            component.IsSet = true; 
        }
    }

    public void ExecuteBatch(AnEntityArchetype<EcsEntity> entityArchetype, int start, int end, CancellationToken token)
    {
        var componentPool = entityArchetype
            .GetComponentPool<ComponentPoolDod<HelloWorldMessageComponent>>(ComponentType.HELLO_WORLD_COMPONENT);

        var span = componentPool.GetSpanOfActiveObjects();
        if (span.IsEmpty)
        {
            Console.WriteLine($"\t empty span: {Name} start: {start} - end: {end}");
            return;
        }

        Console.WriteLine($"\t smaller span: {Name} start: {start} - end: {end} - span length: {span.Length}");
        for (int i = start; i < span.Length && i < end; i++)
        {
            ref var component = ref span[i];

            Console.WriteLine($"\t system: {Name} start: {start} - end: {end} componentId {component.Id}");
            if (component.IsSet) continue;

            Console.WriteLine(component.helloWorldMessage);
            component.IsSet = true;
        }
    }
}

public class FinaliseEntitySystem : ISystem<EcsEntity>
{
    public string Name => "FinaliseEntitySystem";

    public void Execute(AnEntityArchetype<EcsEntity> entityArchetype, CancellationToken token)
    {
        var componentPool = entityArchetype
            .GetComponentPool<ComponentPoolDod<HelloWorldMessageComponent>>(ComponentType.HELLO_WORLD_COMPONENT);
        var span = entityArchetype.GetActiveEntities();

        for (int i = 0; i < span.Length; i++)
        {
            ref var entity = ref span[i];
            var componentId = entity.GetComponentId(ComponentType.HELLO_WORLD_COMPONENT);

            if (componentId == -1) continue;

            ref var component = ref componentPool.GetObjectWithId(componentId);

            if (component.IsSet)
            {
                entityArchetype.FreeEntity(entity.Id);
            }
        }
    }

    public void ExecuteBatch(AnEntityArchetype<EcsEntity> entityArchetype, int start, int end, CancellationToken token)
    {
        var componentPool = entityArchetype
             .GetComponentPool<ComponentPoolDod<HelloWorldMessageComponent>>(ComponentType.HELLO_WORLD_COMPONENT);
        var span = entityArchetype.GetActiveEntities();
        
        if (span.IsEmpty) return;
        
        for (int i = start; i < span.Length && i < end; i++)
        {
            ref var entity = ref span[i];
            var componentId = entity.GetComponentId(ComponentType.HELLO_WORLD_COMPONENT);

            if (componentId == -1) continue;

            ref var component = ref componentPool.GetObjectWithId(componentId);

            if (component.IsSet)
            {
                Console.WriteLine($"\t system: {Name} start: {start} - end: {end} entity: {entity.Id}");
                entityArchetype.FreeEntity(entity.Id);
            }
        }
    }
}

