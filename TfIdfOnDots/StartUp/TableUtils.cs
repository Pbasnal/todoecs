public class TableUtils
{
    public static (bool, int) SearchInArray<T>(T[] array, ref T elementToSearch, Func<T, T, int> comparer)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if(comparer(array[i], elementToSearch) == 0)
            {
                return (true, i);
            }
        }

        return (false, 0);
    }
}