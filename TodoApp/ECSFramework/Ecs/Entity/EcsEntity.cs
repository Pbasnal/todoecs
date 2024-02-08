namespace ECSFramework;

public struct EcsEntity : IEntity
{
    public int Id { get; set; }
    public int[] components;
    public int ProcessedComponents { get; set; }

    public void Init()
    {
        ProcessedComponents = 0;
        components = new int[ComponentType.Length];
        for (int i = 0; i < components.Length; i++)
        {
            components[i] = -1;
        }
    }

    public void MapComponentToEntity<T>(T component) where T : struct, IComponent
    {
        var componentTypeId = ComponentType.GetComponentTypeId(component.GetType());
        components[componentTypeId] = component.Id;
    }

    public void MapComponentToEntity(int componentType, int componentId)
    {
        components[componentType] = componentId;
    }

    public int GetComponentId(int componentType)
    {
        return components[componentType];
    }
}
