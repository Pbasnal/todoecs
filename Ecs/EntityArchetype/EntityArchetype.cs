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
namespace TodoApp
{
    public class EntityArchetype<E> where E : IEntity, new()
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
        protected ConcurrentDictionary<int, E> entities;
        private readonly ConcurrentQueue<E> entityPool;
        private readonly int initialNumberOfEntities;

        /*
        * * The below map contains a component pool mapped to the type of the component.
        * * A component pool is a holder of component array of a single component type.
        */
        private readonly ConcurrentDictionary<Type, IComponentPool> componentsMap;

        public EntityArchetype(int initialNumberOfEntities)
        {
            this.initialNumberOfEntities = initialNumberOfEntities;
            var initialEntities = new List<E>();
            for (int i = 0; i < initialNumberOfEntities; i++)
            {
                initialEntities.Add(new E
                {
                    Index = i
                });
            }
            entityPool = new ConcurrentQueue<E>(initialEntities);
            entities = new ConcurrentDictionary<int, E>();
            componentsMap = new ConcurrentDictionary<Type, IComponentPool>();
        }

        public E CreateEntity()
        {
            if (!entityPool.TryDequeue(out E? entity))
            {
                entity = new E
                {
                    Index = entities.Count + entityPool.Count
                };
                // entityCount.Inc();
            }

            entities.AddOrUpdate(entity.Index, entity, (i, e) => entity);

            return entity;
        }

        public IEnumerable<E> GetActiveEntities()
        {
            foreach (var entity in entities)
            {
                yield return entity.Value;
            }

            yield break;
        }

        public ref T CreateComponentOfEntity<T>(E entity)
          where T : struct, IComponent
        {
            ComponentPool<T> componentPool;
            if (!componentsMap.ContainsKey(typeof(T)))
            {
                componentPool = new ComponentPool<T>(initialNumberOfEntities);
                componentsMap.AddOrUpdate(typeof(T), componentPool, (a, b) => b);
            }

            // _logger.LogDebug("Adding component of type > " + typeof(T));

            componentPool = GetComponentPool<T>();

            return ref componentPool.GetElementAt(entity.Index);
        }

        public void UpdateComponent<T>(E entity, T component) where T : struct, IComponent
        {
            if (!componentsMap.ContainsKey(typeof(T)))
            {
                throw new Exception("Component of requested type doesn't exists for the given entity archetype. Entity: "
                + entity.GetType().FullName + " | Component: " + component.GetType().FullName);
            }
            var componentPool = (ComponentPool<T>)componentsMap[typeof(T)];
            componentPool.UpdateComponent(entity.Index, component);
        }

        public ComponentPool<T> GetComponentPool<T>() where T : struct, IComponent
        {
            return (ComponentPool<T>)componentsMap[typeof(T)];
        }

        /*
        * * We need a way to discard a used entity so that it can be reused. GetResponse is used for that purpose.
        * * 
        * * This function has another purpose. To write an API, we need at least two components, Request component
        * * and Response component. And we also need a way to get the Response component once the request has been 
        * * completed. This function returns the response component and marks the entity for reuse.
        */
        public T GetResponse<T>(E entity) where T : struct, IComponent
        {
            var componentPool = GetComponentPool<T>();
            var response = componentPool.GetElementAt(entity.Index);
            _ = entities.Remove(entity.Index, out _);
            entityPool.Enqueue(entity);

            return response;
        }
    }
}