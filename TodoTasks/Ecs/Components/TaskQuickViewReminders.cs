namespace TodoApp
{
    public struct TaskQuickViewReminders : IComponent
    {
        public int ComponentTypeId() => ComponentType.QUICK_VIEW_REMINDER;

        public string[] taskReminder;
    }
}
