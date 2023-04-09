namespace TodoApp
{
    public class DataStore
    {
        private SortedDictionary<int, string> taskTitles;
        private SortedDictionary<int, TaskEstimateData> taskEstimates;
        private SortedDictionary<int, string> taskReminders;

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
            taskTitles = new SortedDictionary<int, string>();
            taskEstimates = new SortedDictionary<int, TaskEstimateData>();
            taskReminders = new SortedDictionary<int, string>();

            for (int i = 0; i < numberOfTasks; i++)
            {
                taskTitles.Add(i, "task-" + i);
                taskEstimates.Add(i, new TaskEstimateData
                {
                    initialEstimate = DateTime.Now.ToLongDateString(),
                    completedOn = DateTime.Now.AddDays(12).ToLongDateString()
                });
                taskReminders.Add(i, DateTime.Now.AddDays(6).ToLongDateString());
            }
        }

        public Dictionary<int, string> GetTaskTitles(
            int index,
            int count)
        {
            var titles = new Dictionary<int, string>();

            for (int i = index; i < count; i++)
            {
                titles.Add(i, taskTitles[i]);
            }

            return titles;
        }

        public Dictionary<int, TaskEstimateData> GetTaskEstimates(
            int index,
            int count)
        {
            var estimates = new Dictionary<int, TaskEstimateData>();

            for (int i = index; i < count; i++)
            {
                estimates.Add(i, taskEstimates[i]);
            }

            return estimates;
        }

        public Dictionary<int, string> GetTaskReminders(
            int index,
            int count)
        {
            var reminders = new Dictionary<int, string>();

            for (int i = index; i < count; i++)
            {
                reminders.Add(i, taskReminders[i]);
            }

            return reminders;
        }
    }
}