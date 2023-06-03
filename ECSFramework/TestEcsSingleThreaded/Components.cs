namespace ECSFramework;

public struct HelloWorldMessageComponent : IComponent
{
    public bool IsSet { get; set; }
    public int Id { get; set; }

    public int[] Nums { get; set; }

    public ValueString helloWorldMessage;

    public void Init()
    {
        helloWorldMessage.Init();
    }

    public int ComponentTypeId()
    {
        return ComponentType.HELLO_WORLD_COMPONENT;
    }
}
