namespace StartUp;

public static class Algos
{
    public static bool UpdateComponent<T>(int insertAt, T componentValue, ref T[] column)
    {
        if(insertAt < column.Length) {
            column[insertAt] = componentValue;
            return true;
        }
        return false;
    }
}
