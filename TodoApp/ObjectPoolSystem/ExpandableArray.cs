namespace ObjectPoolSystem;

public class ExpandableArray<T> where T : struct, IValueObject
{
    private Dictionary<int, Memory<T>> elementPartitions;

    private Memory<T> memoryArray;

    public int Length;
    private int totalCapacity => partitionSize * partitionCount;

    private readonly int partitionSize;
    private int partitionCount;

    public ExpandableArray(int initialRowCount, int maxRowSize)
    {
        elementPartitions = new Dictionary<int, Memory<T>>();
        var data = new T[initialRowCount * maxRowSize];
        for (int i = 0; i < initialRowCount; i++)
        {
            elementPartitions.Add(i, new T[maxRowSize]);
            data[i].Id = i;
        }

        memoryArray = new Memory<T>(data);  
        this.partitionCount = initialRowCount;
        this.partitionSize = maxRowSize;
    }

    public ref T this[int i]
    {
        get { return ref GetElementAtIndex(i); }
    }

    private ref T GetElementAtIndex(int index)
    {
        //if (index > Length) throw new IndexOutOfRangeException();

        //var partitionKey = GetPartitionKey(index);
        //var elementIndex = GetElementIndexInPartition(index);

        return ref memoryArray.Span[index];
    }

    public void InitElementAtIndex(int index)
    {
        if (index > Length) return;
        if (index == Length) Length++;

        //var partitionKey = GetPartitionKey(index);
        //var elementIndex = GetElementIndexInPartition(index);

        if (index >= totalCapacity) IncreasePartitions();

        memoryArray.Span[index].Id = index;
    }

    private void IncreasePartitions()
    {
        var newPartitionCount = partitionCount * 2;
        for (int i = 0; i < newPartitionCount - partitionCount + 1; i++)
        {
            elementPartitions.Add(partitionCount + i, new T[partitionSize]);
        }

        partitionCount = elementPartitions.Count;
    }

    private int GetPartitionKey(int index) => index / partitionSize;
    private int GetElementIndexInPartition(int index) => index % partitionSize;
}
