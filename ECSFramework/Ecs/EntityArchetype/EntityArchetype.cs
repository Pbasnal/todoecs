/*
* * An Archetype is a specific set of components that are applicable for an entity. For example, to present a 
* * simplified view of a task we only need to show it's title and isDone field but for a complete view of Task,
* * we would need more components such as estimatedData, desciption, reminders etc. Each combination is an Archetype.
* * 
* * Archetype are generally very complicated to implement because we have to handle entities that can change their
* * archetype. But for a APIs, we don't need that funcationality. We only need the logical encapsulation of a set
* * of components.
*/
using ECSFramework.Ecs.System;

namespace ECSFramework;

public abstract class AnEntityArchetype<INIT_COMPONENT, FINAL_COMPONENT>
    where INIT_COMPONENT : struct, IComponent
    where FINAL_COMPONENT : struct, IComponent
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
    protected readonly ComponentPoolDod<EcsEntity> entities;

    private readonly IList<IComponentPool> componentPools;

    public AnEntityArchetype(int initialNumberOfEntities)
    {
        entities = new ComponentPoolDod<EcsEntity>(initialNumberOfEntities);

        var initializerComponents = new ComponentPoolDod<INIT_COMPONENT>(initialNumberOfEntities);
        var finalizerComponents = new ComponentPoolDod<FINAL_COMPONENT>();

        componentPools = GetAllComponentPools(initialNumberOfEntities);
        componentPools.Insert(0, initializerComponents);
        componentPools.Add(finalizerComponents);
    }

    public Memory<EcsEntity> GetActiveEntities()
    {
        return entities.GetActiveObjects();
    }

    public ref EcsEntity CreateEntity(ref INIT_COMPONENT requestData)
    {
        ref var entity = ref entities.GetFreeObject();
        entity.Init();
        requestData.EntityId = entity.Id;

        ref var entityInitializerComponent = ref GetComponentPool<INIT_COMPONENT>().GetFreeObject();
        entityInitializerComponent.CopyFrom(requestData);
        entity.MapComponentToEntity(entityInitializerComponent);

        ref var entityFinalizerComponent = ref GetComponentPool<FINAL_COMPONENT>().GetFreeObject();
        entity.MapComponentToEntity(entityFinalizerComponent);

        return ref entity;
    }

    public void StartSystems(CancellationToken token)
    {
        var systems = GetSystems();
        systems.Insert(0, new EntityInitializerSystem());
        systems.Add(new EntityFinalizerSystem());

        ExecuteSystems(systems, entities.Length, token);
        //Console.WriteLine("Terminating system execution");
    }

    public async Task StartSystemsAsync(int batchSize, CancellationToken token)
    {
        var systems = GetSystems();
        systems.Insert(0, new EntityInitializerSystem());
        systems.Add(new EntityFinalizerSystem());

        await Task.Run(() =>
        {
            ExecuteSystems(systems, batchSize, token);
        }, token);

        //Console.WriteLine("Terminating system execution");
    }

    private void ExecuteSystems(IList<ISystem> systems, int batchSize, CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested && entities.Length > 0)
            {
                foreach (var system in systems)
                {
                    //Console.WriteLine($"\t system: {system.Name} batchSize: {batchSize}");
                    system.Execute(this, batchSize, token);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            throw;
        }
    }

    public void FreeEntity(int entityId)
    {
        ref var entity = ref entities.GetObjectWithId(entityId);
        var initializeComponentTypeId = ComponentType.GetComponentTypeId<EntityInitializerComponent>();

        try
        {
            var initializerId = entity.GetComponentId(initializeComponentTypeId);
            GetComponentPool<INIT_COMPONENT>().ReturnObjectWithId(initializerId);

            var finalizeComponentTypeId = ComponentType.GetComponentTypeId<EntityFinalizerComponent>();
            var finalizerId = entity.GetComponentId(finalizeComponentTypeId);
            GetComponentPool<FINAL_COMPONENT>().ReturnObjectWithId(finalizerId);

            FreeComponentsOfEntity(ref entity);

            entities.ReturnObjectWithId(entityId);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            throw;
        }
    }

    internal ComponentPoolDod<TC> GetComponentPool<TC>() where TC : struct, IComponent
    {
        var componentTypeId = ComponentType.GetComponentTypeId<TC>();
        return (ComponentPoolDod<TC>)componentPools[componentTypeId];
    }

    internal abstract IList<IComponentPool> GetAllComponentPools(int initialNumberOfEntities);
    internal abstract void AddComponentsToEntity(ref EcsEntity entity, ref INIT_COMPONENT requestData);
    internal abstract void AddComponentToEntity<C>(ref EcsEntity entity, C component) where C : struct, IComponent;
    internal abstract void FreeComponentsOfEntity(ref EcsEntity entity);
    internal abstract IList<ISystem> GetSystems();
}