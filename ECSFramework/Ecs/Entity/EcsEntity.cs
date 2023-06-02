namespace ECSFramework;

public struct EcsEntity : IEntity
{
    public int Id { get; set; }
    public int[] components;

    public void Init()
    {
        components = new int[ComponentType.Length]; 
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
