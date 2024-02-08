namespace ECSFramework;

/*
* * A system is a unit of logic that performs some operations on one or multiple types 
* * of components. 
* * Since, a system works on specific type of components and components are stored in
* * contigous arrays, systems experience very less cpu cache misses.
* * 
* * There are other benefits of constructing systems this way.
* * 1. Every system operates on specified components, which means we can parallelize 
* *    most  systems
* * 2. We get architectural benefit as well because systems now have a defined scope
* *    and they naturally follow the Single Responsibility Principle.
*/
internal interface ISystem
{
    string Name { get; }
    void Execute<INIT_COMPONENT, FINAL_COMPONENT>(AnEntityArchetype<INIT_COMPONENT, FINAL_COMPONENT> entityArchetype,
        int batchSize, CancellationToken token)
        where INIT_COMPONENT : struct, IComponent
        where FINAL_COMPONENT : struct, IComponent;
}

public abstract class ASystemBase<IN> : ISystem
    where IN : struct, IComponent
{
    public abstract string Name { get; }

    public void Execute<INIT_COMPONENT, FINAL_COMPONENT>(AnEntityArchetype<INIT_COMPONENT, FINAL_COMPONENT> entityArchetype,
        int batchSize, CancellationToken token)
        where INIT_COMPONENT : struct, IComponent
        where FINAL_COMPONENT : struct, IComponent
    {
        var componentPool = entityArchetype.GetComponentPool<IN>();
        var memory = componentPool.GetActiveObjects();
        if (memory.IsEmpty) return;

        var systemTasks = new List<Task>();
        for (int i = 0; i < memory.Length; i += batchSize)
        {
            var len = memory.Length > i + batchSize ? batchSize : memory.Length - i;
            systemTasks.Add(StartSystem(memory.Slice(i, len)));
        }

        Task.WaitAll(systemTasks.ToArray());
    }

    private async Task StartSystem(Memory<IN> componentsMemory)
    {
        await Task.Factory.StartNew(() =>
        {
            var componentsSpan = componentsMemory.Span;
            for (int i = 0; i < componentsSpan.Length; i++)
            {
                ref var component = ref componentsSpan[i];
                if (component.IsSet) continue; // this is being set by the second system which is an anti pattern.
                                               // Future work to come up with an existance based approach
                ProcessComponent(ref component);
            }            
        });
    }

    protected abstract void ProcessComponent(ref IN inputComponent);
}

public abstract class ASystemBase<IN1, IN2> : ISystem
    where IN1 : struct, IComponent
    where IN2 : struct, IComponent
{
    public abstract string Name { get; }

    public void Execute<INIT_COMPONENT, FINAL_COMPONENT>(AnEntityArchetype<INIT_COMPONENT, FINAL_COMPONENT> entityArchetype,
        int batchSize, CancellationToken token)
        where INIT_COMPONENT : struct, IComponent
        where FINAL_COMPONENT : struct, IComponent
    {
        var input1Pool = entityArchetype.GetComponentPool<IN1>();
        var input1Memory = input1Pool.GetActiveObjects();
        if (input1Memory.IsEmpty) return;

        var input2Pool = entityArchetype.GetComponentPool<IN2>();
        var input2Memory = input2Pool.GetActiveObjects();
        if (input2Memory.IsEmpty) return;

        var systemTasks = new List<Task>();
        for (int i = 0; i < input1Memory.Length; i += batchSize)
        {
            var len = input1Memory.Length > i + batchSize ? batchSize : input1Memory.Length - i;
            systemTasks.Add(StartSystem(input1Memory.Slice(i, len), input2Memory.Slice(i, len)));
        }

        Task.WaitAll(systemTasks.ToArray());
    }

    private async Task StartSystem(Memory<IN1> input1Memory, Memory<IN2> input2Memory)
    {
        await Task.Factory.StartNew(() =>
        {
            var input1Span = input1Memory.Span;
            var input2Span = input2Memory.Span;

            for (int i = 0; i < input1Span.Length; i++)
            {
                ref var component1 = ref input1Span[i];
                ref var component2 = ref input2Span[i];
                if (component1.IsSet && component2.IsSet) continue; // this is being set by the second system which is an anti pattern.
                                               // Future work to come up with an existance based approach
                ProcessComponent(ref component1, ref component2);
            }
        });
    }

    protected abstract void ProcessComponent(ref IN1 component1, ref IN2 component2);
}

public abstract class ASystemWithOutputBase<IN, OUT> : ISystem
    where IN : struct, IComponent
    where OUT : struct, IComponent
{
    public abstract string Name { get; }

    public void Execute<INIT_COMPONENT, FINAL_COMPONENT>(AnEntityArchetype<INIT_COMPONENT, FINAL_COMPONENT> entityArchetype,
        int batchSize, CancellationToken token)
        where INIT_COMPONENT : struct, IComponent
        where FINAL_COMPONENT : struct, IComponent
    {
        var componentPool = entityArchetype.GetComponentPool<IN>();
        var memory = componentPool.GetActiveObjects();
        if (memory.IsEmpty) return;

        var systemTasks = new List<Task>();
        for (int i = 0; i < memory.Length; i += batchSize)
        {
            var len = memory.Length > i + batchSize ? batchSize : memory.Length - i;
            systemTasks.Add(StartSystem(entityArchetype, memory.Slice(i, len)));
        }

        Task.WaitAll(systemTasks.ToArray());
    }

    private async Task StartSystem<INIT_COMPONENT, FINAL_COMPONENT>(AnEntityArchetype<INIT_COMPONENT, FINAL_COMPONENT> entityArchetype,
        Memory<IN> componentsMemory)
        where INIT_COMPONENT : struct, IComponent
        where FINAL_COMPONENT : struct, IComponent
    {
        await Task.Factory.StartNew(() =>
        {
            var entities = entityArchetype.GetActiveEntities().Span;
            var componentsSpan = componentsMemory.Span;
            for (int i = 0; i < componentsSpan.Length; i++)
            {
                ref var component = ref componentsSpan[i];
                if (component.IsSet) continue; // this is being set by the second system which is an anti pattern.
                                               // Future work to come up with an existance based approach
                ref var outputComponent = ref ProcessComponent(ref component);

                var entityId = outputComponent.EntityId;
                ref var entity = ref entities[entityId];
                entityArchetype.AddComponentToEntity(ref entity, outputComponent);
            }
        });
    }

    protected abstract ref OUT ProcessComponent(ref IN inputComponents);
}
