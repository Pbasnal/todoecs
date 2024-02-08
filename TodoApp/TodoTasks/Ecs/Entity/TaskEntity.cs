using ECSFramework;

namespace TodoApp;

public struct TaskEntity : IEntity
{
    public int Id { get; set; }

    public int[] components;

    public TaskEntity()
    {
        Id = 0;
        components = new int[6]; // 6 is the number of component types we have
        for (int i = 0; i < components.Length; i++)
        {
            components[i] = -1;
        }
    }

    public void SetComponent(int componentType, int componentId)
    {
        components[componentType] = componentId;
    }

    public int GetComponentId(int componentType)
    { 
        return components[componentType];
    }
}