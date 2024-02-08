namespace ECSFramework;

public class MessageSetterSystem : ASystemBase<HelloWorldMessageComponent>
{
    public override string Name => "MessageSetterSystem";

    protected override void ProcessComponent(ref HelloWorldMessageComponent component)
    {
        component.helloWorldMessage.Set("Hello world " + component.Id);
    }
}

public class MessageConsolePrintSystem : ASystemBase<HelloWorldMessageComponent, EntityFinalizerComponent>
{
    public override string Name => "MessageConsolePrintSystem";

    protected override void ProcessComponent(ref HelloWorldMessageComponent helloComponent, ref EntityFinalizerComponent finalComponent)
    {
        Console.WriteLine(helloComponent.helloWorldMessage);
        helloComponent.IsSet = true;
        finalComponent.IsSet = true;
    }
}
