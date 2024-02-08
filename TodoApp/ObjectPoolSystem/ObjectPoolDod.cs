namespace ObjectPoolSystem;

public class ObjectPoolDod<T> where T : struct, IValueObject
{
    // can't use list even though it provides continuous allocation on heap with structs
    // because we can't get ref of it's elements.

    private ExpandableMemArray<T> objectPool;
    private int[] poolIndex;

    private int lastInUseObject = -1;

    public ObjectPoolDod(int initialSize = 4096)
    {
        objectPool = new ExpandableMemArray<T>(initialSize);
        poolIndex = new int[initialSize];
    }

    public ref T GetFreeObject()
    {
        var span = objectPool.AsSpan();

        if (lastInUseObject == objectPool.Length)
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

    public void ReturnObject(ref T freedObject)
    {
        var span = objectPool.AsSpan();
        var indexOfFreeObject = poolIndex[freedObject.Id];

        span[indexOfFreeObject] = span[lastInUseObject];
        poolIndex[span[lastInUseObject].Id] = indexOfFreeObject;

        lastInUseObject--;
    }

    public void ReturnObjectWithId(int id)
    {
        var span = objectPool.AsSpan();
        var indexOfFreeObject = id;

        span[indexOfFreeObject] = span[lastInUseObject];
        poolIndex[span[lastInUseObject].Id] = indexOfFreeObject;

        lastInUseObject--;
    }
}
