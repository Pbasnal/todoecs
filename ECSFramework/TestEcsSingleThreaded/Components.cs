namespace ECSFramework;

public struct HelloWorldMessageComponent : IComponent
{
    public bool IsSet { get; set; }
    public int Id { get; set; }
    public int EntityId { get; set; }

    public int[] Nums { get; set; }

    public ValueString helloWorldMessage;

    public void Init(int sizeOfNums)
    {
        IsSet = false;
        Nums = new int[sizeOfNums];
        helloWorldMessage = new ValueString();
    }

    public void CopyFrom(IComponent component)
    {
        if (component is not HelloWorldMessageComponent) return;

        var helloComponent = (HelloWorldMessageComponent)component;
        this.IsSet = helloComponent.IsSet;
        this.EntityId = helloComponent.EntityId;
        this.Nums = helloComponent.Nums;
        this.helloWorldMessage = helloComponent.helloWorldMessage;
    }
}
