using System.Collections.Concurrent;

using TodoApp;

namespace todorest;

public class DataStoreOop
{
    private ConcurrentDictionary<int, TaskData> tasks;

    private static DataStoreOop dataStore;

    public static DataStoreOop GetDataStore(int numberOfTasks)
    {
        if (dataStore == null)
        {
            dataStore = new DataStoreOop(numberOfTasks);
        }
        return dataStore;
    }

    public static DataStoreOop GetDataStore()
    {
        if (dataStore == null)
        {
            throw new Exception("DataStore not initialised");
        }
        return dataStore;
    }

    private DataStoreOop(int numberOfTasks)
    {
        tasks = new ConcurrentDictionary<int, TaskData>();
        for (int i = 0; i < numberOfTasks; i++)
        {
            tasks.TryAdd(i, new TaskData
            {
                id = i,
                title = "task-" + i,
                initialEstimate = DateTime.Now.ToLongDateString(),
                completedOn = DateTime.Now.AddDays(12).ToLongDateString()
            });
        }
    }


    public HashSet<TaskData> GetTaskTitles(
        int index,
        int count)
    {
        var tasksResult = new HashSet<TaskData>();
        for (int i = 0; i < count; i++)
        {
            tasksResult.Add(tasks[i + index]);
        }

        return tasksResult;
    }
}
