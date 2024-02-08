namespace ECSFramework;

public struct EntityInitializerComponent : IComponent
{
    public bool IsSet { get; set; }
    public int Id { get; set; }

    public int LengthToArrayToSort { get; set; }

    public int EntityId { get; set; }

    public void CopyFrom(EntityInitializerComponent component)
    {
        this.IsSet = component.IsSet;
        this.LengthToArrayToSort = component.LengthToArrayToSort;
        this.EntityId = component.EntityId;
    }

    public void CopyFrom(IComponent component)
    {
        if (component is not EntityInitializerComponent) return;

        CopyFrom((EntityInitializerComponent)component);
    }
}
