using Prometheus;

namespace TodoApp
{
    public interface IComponentPool
    {
        // void Initialise(int numberOfElements);
        bool IsEmpty();
    }

    public class ComponentPool<T> : IComponentPool
        where T : struct, IComponent
    {
        private int poolUnitSize = 100;

        private Dictionary<int, T[]> componentPool;

        private static readonly Counter componentCount = Metrics
                .CreateCounter("component_count", "Number of components.",
                labelNames: new[] { "component" });

        public bool IsEmpty() => componentPool.Count * poolUnitSize == 0;

        public ComponentPool()
        {
            componentPool = new Dictionary<int, T[]>
            {
                { 0, new T[poolUnitSize] }
            };
            componentCount.WithLabels(typeof(T).Name).Inc(poolUnitSize);
        }

        public ref T GetElementAt(int index)
        {
            var currentPoolKey = GetPoolKey(index);
            var elementIndex = GetElementIndex(index);

            if (!componentPool.ContainsKey(currentPoolKey))
            {
                componentPool.Add(currentPoolKey, new T[poolUnitSize]);
                componentCount.Inc(poolUnitSize);
            }

            return ref componentPool[currentPoolKey][elementIndex];
        }

        public void UpdateComponent(int index, T component)
        {
            componentPool[GetPoolKey(index)][GetElementIndex(index)] = component;
        }


        private int GetPoolKey(int index) => index / poolUnitSize;
        private int GetElementIndex(int index) => index % poolUnitSize;
    }
}