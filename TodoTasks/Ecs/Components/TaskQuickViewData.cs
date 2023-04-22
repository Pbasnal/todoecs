namespace TodoApp
{
    public struct TaskQuickViewData : IComponent
    {
        public static int componentId;
        public int ComponentTypeId() => ComponentType.QUICK_VIEW_DATA;

        public string[] taskTitle;
        public TaskEstimates[] taskEstimates;
        public string[] taskReminder;
        public bool IsSet { get; set; }
    }
}
