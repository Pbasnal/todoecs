using todorest;

namespace TodoApp
{
    /*
    * * A simple class to emulate data storage. It contains data in arrays since that can help in 
    * * DOD based programming.
    ! * We are using array of strings. However, since strings are reference types, they will be 
    ! * allocated on the heap at random locations reducing the effects of our optimisation.
    */
    public class DataStore
    {
        private readonly string[] taskTitles;
        private readonly TaskEstimateData[] taskEstimates;
        private readonly string[] taskReminders;

        private static DataStore dataStore;

        public static DataStore GetDataStore(int numberOfTasks)
        {
            if (dataStore == null)
            {
                dataStore = new DataStore(numberOfTasks);
            }
            return dataStore;
        }

        public static DataStore GetDataStore()
        {
            if (dataStore == null)
            {
                throw new Exception("DataStore not initialised");
            }
            return dataStore;
        }

        private DataStore(int numberOfTasks)
        {
            taskTitles = new string[numberOfTasks];
            taskEstimates = new TaskEstimateData[numberOfTasks];
            // taskReminders = new string[numberOfTasks];

            // In real scenario, we would want to use insertion sort to load the data.
            var taskArray = new TaskEstimateData[numberOfTasks];
            for (int i = 0; i < numberOfTasks; i++)
            {
                taskTitles[i] = "task-" + i;
                SetTaskEstimateData(ref taskArray[i]);
                // taskReminders[i] = DateTime.Now.AddDays(6).ToLongDateString();
            }
        }

        private static void SetTaskEstimateData(ref TaskEstimateData taskEstimate)
        {
            taskEstimate.initialEstimate = DateTime.Now.ToLongDateString();
            taskEstimate.completedOn = DateTime.Now.AddDays(12).
                            ToLongDateString();
        }

        public Memory<string> GetTaskTitles(
            int index,
            int count)
        {
            return new Memory<string>(taskTitles, index, count);
        }

        // public Dictionary<int, TaskEstimateData> GetTaskEstimates(
        //     int index,
        //     int count)
        // {
        //     var estimates = new Dictionary<int, TaskEstimateData>();

        //     for (int i = index; i < count; i++)
        //     {
        //         estimates.Add(i, taskEstimates[i]);
        //     }

        //     return estimates;
        // }

        // public Dictionary<int, string> GetTaskReminders(
        //     int index,
        //     int count)
        // {
        //     var reminders = new Dictionary<int, string>();

        //     for (int i = index; i < count; i++)
        //     {
        //         reminders.Add(i, taskReminders[i]);
        //     }

        //     return reminders;
        // }
    }
}