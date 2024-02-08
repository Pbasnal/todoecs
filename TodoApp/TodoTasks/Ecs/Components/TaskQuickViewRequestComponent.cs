using ECSFramework;

namespace TodoApp
{
    public struct TaskQuickViewRequestComponent : IComponent
    {
        public int ComponentTypeId() => ComponentType.QUICK_VIEW_REQUEST;

        public int page;
        public int numberOfTasks;
        public int Id { get; set; }
        public bool IsSet { get; set; }
    }
}
