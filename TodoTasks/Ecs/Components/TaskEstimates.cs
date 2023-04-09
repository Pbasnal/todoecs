namespace TodoApp
{
    public struct TaskEstimates : IComponent
    {
        public int ComponentTypeId() => ComponentType.TASK_ESTIMATES;

        public string initialEstimate;
        public string completedOn;
    }
}
