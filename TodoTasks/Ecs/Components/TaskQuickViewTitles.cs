using todorest;

namespace TodoApp
{
    public struct TaskQuickViewTitles : IComponent
    {
        public int ComponentTypeId() => ComponentType.QUICK_VIEW_TASK_TITLES;

        public string[] taskTitle;

        public bool IsSet { get; set; }
    }
}
