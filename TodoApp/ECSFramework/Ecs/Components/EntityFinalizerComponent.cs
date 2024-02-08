namespace ECSFramework;

public struct EntityFinalizerComponent : IComponent
{
    public bool IsSet { get; set; }
    public int Id { get; set; }
    public int EntityId { get; set; }

    public void CopyFrom(EntityFinalizerComponent component)
    {
        this.IsSet = component.IsSet;
        this.EntityId = component.EntityId;
    }

    public void CopyFrom(IComponent component)
    {
        if (component is not EntityFinalizerComponent) return;

        CopyFrom((EntityFinalizerComponent)component);
    }
}
