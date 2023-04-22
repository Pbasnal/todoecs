namespace TodoApp
{
    public struct TaskQuickViewRequestComponent : IComponent
    {
        public int ComponentTypeId() => ComponentType.QUICK_VIEW_REQUEST;

        public int page;
        public int numberOfTasks;

        public bool IsSet { get; set; }
    }
}
