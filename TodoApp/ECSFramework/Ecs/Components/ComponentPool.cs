namespace ECSFramework
{
    /*
    * * ComponentPool is created for each type of component. It holds the list of all
    * * components of one type together and manages them.
    * * The below implementation creates multiple arrays of size 'poolUnitSize'. This 
    * * adds flexibility of increasing and decreasing the pool size as per requirement.
    * * 
    * * Since components are value types. They are passed by value as method arguments
    * * and as method return values. This basicaly means every time a component crosses 
    * * method scope, a copy of the component is created. This can cause huge performance
    * * and memory overhead. It has another side effect that any updates made to the 
    * * component by systems will not get reflected.
    * * 
    * * To resolve above mentioned problem, the implementation passes components with the
    * * 'ref' keyword. This allows us to pass reference of the component instead of 
    * * passing a copy.
    */
    public interface IComponentPool
    {
        bool IsEmpty();
        void SetComponentEmpty(int index);
        void ReturnObjectWithId(int index);
        int GetIndexOfFreeObject();
    }

    //public class EmptyComponentPool : IComponentPool
    //{
    //    public bool IsEmpty() => true;

    //    public void SetComponentEmpty(int index)
    //    {
    //    }

    //    public void ReturnObjectWithId(int index) { }
    //}


    //public class ComponentPool<T> : IComponentPool
    //    where T : struct, IComponent
    //{
    //    private int poolUnitSize = 100;

    //    private Dictionary<int, T[]> componentPool;

    //    public bool IsEmpty() => componentPool.Count * poolUnitSize == 0;

    //    public ComponentPool(int initialNumberOfComponents)
    //    {
    //        componentPool = new Dictionary<int, T[]>();
    //        int numberOfPools = initialNumberOfComponents / poolUnitSize;

    //        for (int i = 0; i < numberOfPools; i++)
    //        {
    //            componentPool.Add(i, new T[poolUnitSize]);
    //        }
    //    }

    //    public ref T CreateComponentForEntityId(int entityId)
    //    {
    //        var currentPoolKey = GetPoolKey(entityId);
    //        var elementIndex = GetElementIndex(entityId);

    //        if (!componentPool.ContainsKey(currentPoolKey))
    //        {
    //            componentPool.Add(currentPoolKey, new T[poolUnitSize]);
    //        }

    //        ref var component = ref componentPool[currentPoolKey][elementIndex];
    //        component.IsSet = false;
    //        component.IsEmpty = false;

    //        return ref component;
    //    }

    //    public ref T GetElementAt(int index)
    //    {
    //        var currentPoolKey = GetPoolKey(index);
    //        var elementIndex = GetElementIndex(index);

    //        if (!componentPool.ContainsKey(currentPoolKey))
    //        {
    //            componentPool.Add(currentPoolKey, new T[poolUnitSize]);
    //        }

    //        ref var component = ref componentPool[currentPoolKey][elementIndex];
    //        component.IsSet = false;
    //        return ref component;
    //    }

    //    public ref T GetNextActiveComponents(int index)
    //    {
    //        var poolKey = GetPoolKey(index);
    //        var componentKey = GetElementIndex(index);

    //        ref T component = ref componentPool[poolKey][componentKey];

    //        if (!component.IsEmpty && !component.IsSet) return ref component;

    //        for (int i = poolKey; i < componentPool.Count; i++)
    //        {
    //            for (int j = componentKey; j < componentPool[poolKey].Length; j++)
    //            {
    //                component = ref componentPool[poolKey][j];
    //                if (!component.IsEmpty && !component.IsSet) break;
    //            }
    //            componentKey = 0;
    //        }

    //        return ref component;
    //    }

    //    public void UpdateComponent(int index, T component)
    //    {
    //        componentPool[GetPoolKey(index)][GetElementIndex(index)] = component;
    //    }

    //    public void SetComponentEmpty(int index)
    //    {
    //        ref var component = ref GetElementAt(index);
    //        component.IsSet = false;
    //        component.IsEmpty = true;
    //    }

    //    private int GetPoolKey(int index) => index / poolUnitSize;
    //    private int GetElementIndex(int index) => index % poolUnitSize;
    //}
}