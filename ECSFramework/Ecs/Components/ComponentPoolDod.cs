using ObjectPoolSystem;

namespace ECSFramework;

public class ComponentPoolDod<T> : IComponentPool
    where T : struct, IValueObject
{
    // can't use list even though it provides continuous allocation on heap with structs
    // because we can't get ref of it's elements.
    private ExpandableMemArray<T> objectPool;
    private int[] poolIndex;

    private int lastInUseObject = -1;

    public int Length => lastInUseObject + 1;

    public ComponentPoolDod(int initialSize = 4096)
    {
        objectPool = new ExpandableMemArray<T>(initialSize);
        poolIndex = new int[initialSize];

        var indexSpan = poolIndex.AsSpan();

        var initialEntities = objectPool.AsSpan();
        for (int i = 0; i < initialSize; i++)
        {
            initialEntities[i].Id = i;
            indexSpan[i] = i;
        }
    }

    public Span<T> GetSpanOfActiveObjects()
    {
        if (lastInUseObject == -1)
        {
            return Span<T>.Empty;
        }
        return objectPool.AsSpan().Slice(0, lastInUseObject + 1);
    }

    public ref T GetFreeObject()
    {
        var span = objectPool.AsSpan();

        if (lastInUseObject == objectPool.Length - 1)
        {
            span = objectPool.IncreaseCapacity();
            Array.Resize(ref poolIndex, span.Length);
        }

        lastInUseObject++;
        ref T objectToReturn = ref span[lastInUseObject];
        objectToReturn.Id = lastInUseObject;

        poolIndex[lastInUseObject] = lastInUseObject;

        return ref objectToReturn;
    }

    public ref T GetObjectWithId(int objectId)
    {
        var objectIndex = poolIndex[objectId];
        return ref objectPool.AsSpan()[objectIndex];
    }

    public void ReturnObject(ref T freedObject)
    {
        var span = objectPool.AsSpan();
        var indexOfFreeObject = poolIndex[freedObject.Id];

        span[indexOfFreeObject] = span[lastInUseObject];
        poolIndex[span[lastInUseObject].Id] = indexOfFreeObject;

        span[lastInUseObject].Id = indexOfFreeObject;

        lastInUseObject--;
    }

    public void ReturnObjectWithId(int id)
    {
        if (lastInUseObject == -1)
        {
            return;
        }

        var span = objectPool.AsSpan();
        var indexOfFreeObject = id;

        span[indexOfFreeObject] = span[lastInUseObject];
        poolIndex[span[lastInUseObject].Id] = indexOfFreeObject;

        lastInUseObject--;
    }

    public bool IsEmpty()
    {
        return lastInUseObject == 0;
    }

    public void SetComponentEmpty(int index)
    {
        ReturnObjectWithId(index);
    }
}
