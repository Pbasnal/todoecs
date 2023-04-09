using System.Collections.Concurrent;
using System.Collections.Generic;
using Prometheus;

namespace TodoApp
{
    public class EntityArchetype<E> where E : IEntity, new()
    {
        protected ConcurrentDictionary<int, E> entities;
        private readonly ConcurrentQueue<E> entityPool;
        // private readonly ILogger _logger;

        private static readonly Counter entityCount = Metrics
                .CreateCounter("entity_count", "Number of entities.");

        private readonly ConcurrentDictionary<Type, IComponentPool> componentsMap;

        public EntityArchetype()
        {
            entityPool = new ConcurrentQueue<E>();
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
                entityCount.Inc();
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


        // public E GetEntity(int i)
        // {
        // if (i >= entities.Count)
        // {
        //     var startingCountOfEntities = entities.Count;
        //     int elementsToAdd = i - entities.Count + 1;
        //     for (int j = 0; j < elementsToAdd; j++)
        //     {
        //         entities.Add(new E
        //         {
        //             Index = startingCountOfEntities + j
        //         });
        //         entityCount.Inc();
        //     }
        // }
        // return entities[i];
        // }

        public ref T CreateComponentOfEntity<T>(E entity)
          where T : struct, IComponent
        {
            ComponentPool<T> componentPool;
            if (!componentsMap.ContainsKey(typeof(T)))
            {
                componentPool = new ComponentPool<T>();
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

        public T GetResponse<T>(E entity) where T : struct, IComponent
        {
            var componentPool = GetComponentPool<T>();
            var response = componentPool.GetElementAt(entity.Index);

            if (!entities.TryRemove(entity.Index, out E value))
            {
                entities.Remove(entity.Index, out value);
            }
            entityPool.Enqueue(entity);

            return response;
        }
    }
}