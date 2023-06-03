using System.Collections.Concurrent;

/*
* * An Archetype is a specific set of components that are applicable for an entity. For example, to present a 
* * simplified view of a task we only need to show it's title and isDone field but for a complete view of Task,
* * we would need more components such as estimatedData, desciption, reminders etc. Each combination is an Archetype.
* * 
* * Archetype are generally very complicated to implement because we have to handle entities that can change their
* * archetype. But for a APIs, we don't need that funcationality. We only need the logical encapsulation of a set
* * of components.
*/
namespace ECSFramework;

public abstract class AnEntityArchetype<E>
    where E : struct, IEntity
{
    /*
    * * This code is using object pooling pattern to reuse entities which have been processed
    * * already. We are also using ConcurrentDictionary to maintain consisteny and integrity 
    * * of the data. Since 1000s of requests will be creating entities in parallel, we need
    * * concurrency management.
    * *
    * * We store all entities in a queue first. When a request for an entity comes, if the queue
    * * has entities, we dequeue one entity and insert it in the dictionary. Dictionary contains 
    * * all the active entities. 
    * * If queue is empty, then we create a new entity.
    * * 
    * * Future plan is to abstract object pooling into a separate module and use it for all most 
    * * of the data objects that we need.
    */
    protected ComponentPoolDod<E> entities;
    private ComponentPoolDod<BufferedEntityComponent> bufferedEntityPool;

    public AnEntityArchetype(int initialNumberOfEntities)
    {
        entities = new ComponentPoolDod<E>(initialNumberOfEntities);
        bufferedEntityPool = new ComponentPoolDod<BufferedEntityComponent>(initialNumberOfEntities);
    }

    public ref E CreateEntity()
    {
        ref var entity = ref entities.GetFreeObject();
        entity.Init();
        return ref entity;
    }

    public void FreeEntity(int entityId)
    {
        ref var entity = ref entities.GetObjectWithId(entityId);

        FreeComponentsOfEntity(ref entity);

        var componentId = entity.GetComponentId(ComponentType.BUFFERED_ENTITY_COMPONENT);
        bufferedEntityPool.ReturnObjectWithId(componentId);

        entities.ReturnObjectWithId(entityId);
    }

    public Memory<E> GetActiveEntities()
    {
        return entities.GetActiveObjects();
    }

    public void StartSystems(CancellationToken token)
    {
        var systems = GetSystems();

        while (!token.IsCancellationRequested
            && entities.Length > 0)
        {
            //Console.WriteLine($"Iterating systems for {entities.Length} entities");
            BufferedEntitySystem();
            foreach (var system in systems)
            {
                system.Execute(this, token);
            }
        }

        //Console.WriteLine("Terminating system execution");
    }

    public async Task StartSystemsAsync(int batchSize, CancellationToken token)
    {
        var systems = GetSystems();

        await Task.Run(() =>
        {
            while (!token.IsCancellationRequested
                && entities.Length > 0)
            {
                BufferedEntitySystem();
                //Console.WriteLine($"Starting system executions for {entities.Length} entities");
                StartTask(systems, batchSize, token);
                //Console.WriteLine($"Iterating systems for {entities.Length} entities");
                //break;
            }
        }, token);

        //Console.WriteLine("Terminating system execution");
    }

    private void StartTask(ISystem<E>[] systems,
        int batchSize,
        CancellationToken token)
    {
        foreach (var system in systems)
        {
            //Console.WriteLine($"\t system: {system.Name} batchSize: {batchSize}");
            system.ExecuteBatch(this, batchSize, token);
        }
    }

    public ref E BuildEntity(int requestData)
    {
        ref var entity = ref CreateEntity();
        //AddComponentsToEntity(entity);

        ref var bufferedEntity = ref bufferedEntityPool.GetFreeObject();
        bufferedEntity.EntityId = entity.Id;
        bufferedEntity.RequestData = requestData;
        entity.SetComponent(bufferedEntity.ComponentTypeId(), bufferedEntity.Id);

        return ref entity;
    }

    private void BufferedEntitySystem()
    {
        //Console.WriteLine($"Executing buffer system to add components");
        var activeEntitySpan = bufferedEntityPool.GetActiveObjects().Span;
        var entitySpan = entities.GetActiveObjects().Span;

        for (int i = 0; i < activeEntitySpan.Length; i++)
        {
            ref var bufferComponent = ref activeEntitySpan[i];

            if (bufferComponent.IsSet) continue;

            ref var entity = ref entitySpan[bufferComponent.EntityId];

            AddComponentsToEntity(ref entity, bufferComponent.RequestData);

            bufferComponent.IsSet = true;
        }
    }

    internal abstract void AddComponentsToEntity(ref E entity, int requestData);

    public abstract ComponentPoolDod<TC> GetComponentPool<TC>(int componentType) where TC : struct, IComponent;

    protected abstract void FreeComponentsOfEntity(ref E entity);

    protected abstract ISystem<E>[] GetSystems();
}