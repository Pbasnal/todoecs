using ECSFramework;

namespace TodoApp
{

    public struct TaskEstimates : IComponent
    {
        public int ComponentTypeId() => ComponentType.TASK_ESTIMATES;

        public string initialEstimate;
        public string completedOn;
        public bool IsSet { get; set; }
        public int Id { get; set; }
    }
}
