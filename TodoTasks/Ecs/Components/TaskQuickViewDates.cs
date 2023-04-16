namespace TodoApp
{
    public struct TaskQuickViewDates : IComponent
    {
        public int ComponentTypeId() => ComponentType.QUICK_VIEW_TASK_DATES;

        public string[] taskDate;
        public bool IsSet { get; set; }
    }
}
