using ECSFramework;

namespace TodoApp
{
    public struct TaskQuickViewReminders : IComponent
    {
        public int Id { get; set; }
        public int ComponentTypeId() => ComponentType.QUICK_VIEW_REMINDER;

        public string[] taskReminder;
        public bool IsSet { get; set; }
    }
}
