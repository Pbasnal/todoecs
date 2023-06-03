using System.Runtime.InteropServices;

namespace ObjectPoolSystem;

public struct MemArray<T> where T : struct
{
    private Memory<T> memoryArray;

    public MemArray(int size)
    {
        var data = new T[size];
        memoryArray = new Memory<T>(data);
    }

    public MemArray(T[] startingArray)
    {
        memoryArray = new Memory<T>(startingArray);
    }

    public Span<T> AsSpan() => memoryArray.Span;
}

public struct ExpandableMemArray<T> where T : struct
{
    private T[] array;
    private Memory<T> memoryArray;

    public int Length;

    public ExpandableMemArray(int size)
    {
        array = new T[size];
        memoryArray = new Memory<T>(array);
        Length = size;
    }

    public Span<T> AsSpan() => memoryArray.Span;
    public Memory<T> AsMemory() => memoryArray;

    public Span<T> IncreaseCapacity()
    {

        CopyArray();
        Length *= 2;

        return AsSpan();
    }


    private void CopyArray()
    {
        var newMemArray = new Memory<T>(new T[Length * 2]);
        memoryArray.TryCopyTo(newMemArray);
        memoryArray = newMemArray;
    }
}