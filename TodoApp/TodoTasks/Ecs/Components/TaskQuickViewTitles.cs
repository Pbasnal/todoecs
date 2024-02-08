using System.Text;

using ECSFramework;

namespace TodoApp;

public struct TaskQuickViewTitles : IComponent
{
    public int ComponentTypeId() => ComponentType.QUICK_VIEW_TASK_TITLES;
    public int Id { get; set; }
    //public Memory<string> taskTitle;
    public Memory<string> taskTitle;

    public bool IsSet { get; set; }
}

// will use later
