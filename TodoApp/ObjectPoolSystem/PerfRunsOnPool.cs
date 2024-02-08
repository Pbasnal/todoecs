using BenchmarkDotNet.Attributes;

namespace ObjectPoolSystem;

public class PoolableObject
{
    public int id;
    public string title;
    public string description;
    public AnotherObject anotherObject;
}

public class AnotherObject
{
    public int id;
    public string title;
    public string description;
}

[MemoryDiagnoser]
public class PerfRunsOnPool
{
    private int numberOfItems = 100000; //100k

    [Benchmark]
    public void RunForOopPool()
    {
        var objectPool = new ObjectPoolOop<PoolableObject>();
        var random = new Random();
        var mapOfObjects = new List<PoolableObject>(numberOfItems);

        for (int i = 0; i < numberOfItems; i++)
        {
            var objectToUse = objectPool.GetObject();
            if (objectToUse == null)
            {
                Console.WriteLine("It's null");
            }
            mapOfObjects.Add(objectToUse);

            if (random.Next(mapOfObjects.Count) % 1000 == 0)
            {
                var randomIndex = random.Next(mapOfObjects.Count);
                var objectToRemove = mapOfObjects[randomIndex];
                objectPool.ReturnObject(objectToRemove);
                mapOfObjects.Remove(objectToRemove);
            }
        }

        foreach (var item in mapOfObjects)
        {
            objectPool.ReturnObject(item);
        }
    }


    [Benchmark]
    public void RunForDodPool()
    {
        var objectPool = new ObjectPoolDod<PoolableObjectDod>();
        var random = new Random();
        var mapOfObjects = new List<int>(numberOfItems);

        for (int i = 0; i < numberOfItems; i++)
        {
            ref var objectToUse = ref objectPool.GetFreeObject();
            mapOfObjects.Add(objectToUse.Id);

            if (random.Next(mapOfObjects.Count) % 1000 == 0)
            {
                var randomIndex = random.Next(mapOfObjects.Count);
                var objectToRemove = mapOfObjects[randomIndex];
                objectPool.ReturnObjectWithId(objectToRemove);
                mapOfObjects.Remove(objectToRemove);
            }
        }

        foreach (var item in mapOfObjects)
        {
            objectPool.ReturnObjectWithId(item);
        }
    }
}

public struct PoolableObjectDod : IValueObject
{
    public int Id { get; set; }
    public string title;
    public string description;
    public AnotherObjectDod anotherObject;
}

public struct AnotherObjectDod
{
    public int id;
    public string title;
    public string description;
}