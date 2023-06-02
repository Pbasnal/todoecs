using ECSFramework;

using todorest;

namespace TodoApp
{
    public struct TaskQuickViewResponseComponent : IComponent
    {
        public int ComponentTypeId() => ComponentType.QUICK_VIEW_RESPONSE;

        public TaskQuickViewCard[] taskQuickViewCards;
        public int Id { get; set; }
        public bool IsSet { get; set; }

    }

    public struct TaskQuickViewCard 
    {
        public string title;
    }
}
