namespace ECSFramework;

public struct BufferedEntityComponent : IComponent
{
    public bool IsSet { get; set; }
    public int Id { get; set; }

    public int RequestData { get; set; }

    public int EntityId { get; set; }

    public int ComponentTypeId()
    {
        return ComponentType.BUFFERED_ENTITY_COMPONENT;
    }
}
