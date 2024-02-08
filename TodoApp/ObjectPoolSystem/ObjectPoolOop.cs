using System.Collections.Concurrent;

namespace ObjectPoolSystem;

public class ObjectPoolOop<T> where T : new()
{
    private HashSet<T> objectsInUse;
    private ConcurrentQueue<T> availableObjectHeap;

    public ObjectPoolOop(int initialSize = 100)
    {
        objectsInUse = new HashSet<T>();
        availableObjectHeap = new ConcurrentQueue<T>();

        for (int i = 0; i < initialSize; i++)
        {
            availableObjectHeap.Enqueue(new T());
        }
    }

    public T GetObject()
    {
        T availableObject;
        if (availableObjectHeap.IsEmpty)
        {
            availableObject = new T();
            _ = objectsInUse.Add(availableObject);
        }
        else if (!availableObjectHeap.TryDequeue(out availableObject))
        {
            throw new Exception("Failed to dequeue object");
        }

        return availableObject;
    }

    public void ReturnObject(T freedObject)
    {
        availableObjectHeap.Enqueue(freedObject);
        objectsInUse.Remove(freedObject);        
    }
}