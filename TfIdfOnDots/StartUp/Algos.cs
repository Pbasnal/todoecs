namespace StartUp;

public static class ArrayAlgos
{
    public static bool UpdateComponent<T>(int insertAt, T componentValue, ref T[] column)
    {
        if (insertAt < column.Length)
        {
            column[insertAt] = componentValue;
            return true;
        }
        return false;
    }

    public static bool ShiftLeft<T>(int startingIndex, int arrayLength, Action<int, int> copyAction)
    {
        if (startingIndex >= arrayLength) return false;
        
        for (int i = startingIndex; i < arrayLength; i++)
        {
            copyAction(i, i + 1);
        }
        return false;
    }
}
