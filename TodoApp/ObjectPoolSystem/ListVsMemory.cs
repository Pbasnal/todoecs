using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace ObjectPoolSystem;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class ListVsMemory
{
    private List<DataHolder> listWithClass;
    private Memory<DataHolder> memoryWithClass;

    private List<DataStore> listWithStruct;
    private HashSet<DataStore> hashSetWithStruct;
    private Memory<DataStore> memoryWithStruct;

    private DataStore[] arrayOfStruct;

    private ExpandableArray<DataStore> expandableArray;

    private MemArray<DataStore> memArray;

    private ExpandableMemArray<DataStore> exMemArray;

    private int numberOfItems = 100000; //100k

    [Benchmark]
    public void ListWithClass()
    {
        listWithClass = new List<DataHolder>();
        for (int i = 0; i < numberOfItems; i++)
        {
            listWithClass.Add(new DataHolder(i));
        }

        for (int i = 0; i < numberOfItems; i++)
        {
            if (i == listWithClass[i].child.id)
            {
                listWithClass[i].child.id *= 2;
            }
            else
            {
                listWithClass[i].child.id += 3;
            }
        }
    }

    [Benchmark]
    public void ListWithStruct()
    {
        listWithStruct = new List<DataStore>();
        for (int i = 0; i < numberOfItems; i++)
        {
            listWithStruct.Add(new DataStore(i));
        }

        int id;
        for (int i = 0; i < numberOfItems; i++)
        {
            id = listWithStruct[i].child.id;
            if (listWithStruct[i].child.id % 2 == 0)
            {
                listWithStruct[i].SetId(id + 3);
            }
            else
            {
                listWithStruct[i].SetId(id * 2);
            }
        }
    }

    [Benchmark]
    public void HashSetWithStruct()
    {
        hashSetWithStruct = new HashSet<DataStore>();
        for (int i = 0; i < numberOfItems; i++)
        {
            hashSetWithStruct.Add(new DataStore(i));
        }

        int id;
        foreach (var item in hashSetWithStruct)
        {
            id = item.child.id;
            if (item.child.id % 2 == 0)
            {
                item.SetId(id + 3);
            }
            else
            {
                item.SetId(id * 2);
            }
        }
    }

    [Benchmark]
    public void SpanWithClass()
    {
        DataHolder[] data = new DataHolder[numberOfItems];
        for (int i = 0; i < numberOfItems; i++)
        {
            data[i] = new DataHolder(i);
        }
        memoryWithClass = new Memory<DataHolder>(data);
        Span<DataHolder> spanWithClass = memoryWithClass.Span;

        for (int i = 0; i < numberOfItems; i++)
        {
            if (i == spanWithClass[i].child.id)
            {
                spanWithClass[i].child.id *= 2;
            }
            else
            {
                spanWithClass[i].child.id += 3;
            }
        }
    }

    [Benchmark]
    public void SpanWithStruct()
    {
        DataStore[] data = new DataStore[numberOfItems];
        for (int i = 0; i < numberOfItems; i++)
        {
            data[i].SetId(i);
        }
        memoryWithStruct = new Memory<DataStore>(data);
        var spanWithStruct = memoryWithStruct.Span;

        for (int i = 0; i < numberOfItems; i++)
        {
            if (i == spanWithStruct[i].child.id)
            {
                spanWithStruct[i].child.id *= 2;
            }
            else
            {
                spanWithStruct[i].child.id += 3;
            }
        }
    }

    [Benchmark]
    public void ArrayOfStructOnStack()
    {
        Span<DataStore> data = stackalloc DataStore[numberOfItems];
        //DataStore[] array = new DataStore[numberOfItems];
        //var data = array.AsSpan();

        for (int i = 0; i < numberOfItems; i++)
        {
            data[i].SetId(i);
        }

        for (int i = 0; i < numberOfItems; i++)
        {
            if (data[i].child.id % 2 == 0)
            {
                data[i].child.id += 3;
            }
            else
            {
                data[i].child.id *= 2;
            }
        }
    }

    [Benchmark]
    public void MemArraysInDictionary()
    {
        expandableArray = new ExpandableArray<DataStore>(numberOfItems / 1000, 1000);
        for (int i = 0; i < numberOfItems; i++)
        {
            expandableArray.InitElementAtIndex(i);
        }

        for (int i = 0; i < numberOfItems; i++)
        {
            ref var data = ref expandableArray[i];
            if (i == data.child.id)
            {
                data.child.id *= 2;
            }
            else
            {
                data.child.id += 3;
            }
        }
    }

    [Benchmark]
    public void MemArrayAsSpan()
    {
        memArray = new MemArray<DataStore>(numberOfItems);
        var memSpan = memArray.AsSpan();

        for (int i = 0; i < numberOfItems; i++)
        {
            memSpan[i].child.id = i;
        }

        for (int i = 0; i < numberOfItems; i++)
        {
            if (memSpan[i].child.id == i)
            {
                memSpan[i].child.id *= 2;
            }
            else
            {
                memSpan[i].child.id += 3;
            }
        }
    }

    [Benchmark]
    public void ExpandableMemArray()
    {
        exMemArray = new ExpandableMemArray<DataStore>(32768);

        var memSpan = exMemArray.AsSpan();
        for (int i = 0; i < numberOfItems; i++)
        {
            if (i == exMemArray.Length)
            {
                memSpan = exMemArray.IncreaseCapacity();
            }
            memSpan[i].child.id = i;
        }

        for (int i = 0; i < numberOfItems; i++)
        {
            if (memSpan[i].child.id == i)
            {
                memSpan[i].child.id *= 2;
            }
            else
            {
                memSpan[i].child.id += 3;
            }
        }
    }
}


public class DataHolder
{
    public DataHolderChild child;

    public DataHolder(int id)
    {
        child = new DataHolderChild(id);

    }
}

public class DataHolderChild
{
    public int id;

    public DataHolderChild(int id)
    {
        this.id = id;
    }
}

public struct DataStore : IValueObject
{
    public int Id { get; set; }

    public DataStoreChild child;

    public DataStore(int id)
    {
        Id = id;
        child.id = id;
    }

    public void SetId(int id)
    {
        child.id = id;
    }
}

public struct DataStoreChild
{
    public int id;
}