namespace ECSFramework;

public class TestArchetype : AnEntityArchetype<EcsEntity>
{
    private IComponentPool[] componentPools;

    public TestArchetype(int initialNumberOfEntities) : base(initialNumberOfEntities)
    {
        componentPools = new IComponentPool[] {
            new ComponentPoolDod<HelloWorldMessageComponent>(initialNumberOfEntities)
        };
    }

    public override TC GetComponentPool<TC>(int componentType)
    {
        return componentType switch
        {
            ComponentType.HELLO_WORLD_COMPONENT => (TC)componentPools[0],
            _ => (TC)componentPools[0],
        };
    }

    internal override void AddComponentsToEntity(ref EcsEntity entity)
    {
        var dataComponentPool = (ComponentPoolDod<HelloWorldMessageComponent>)componentPools[0];
        ref var helloWorldMessageComponent = ref dataComponentPool.GetFreeObject();
        helloWorldMessageComponent.Init();
        entity.SetComponent(helloWorldMessageComponent.ComponentTypeId(), helloWorldMessageComponent.Id);
    }

    protected override ISystem<EcsEntity>[] GetSystems()
    {
        return new ISystem<EcsEntity>[] {
            new MessageSetterSystem(),
            new MessageConsolePrintSystem(),
            new FinaliseEntitySystem()
        };
    }

    protected override void FreeComponentsOfEntity(ref EcsEntity entity)
    {
        var dataComponentPool = (ComponentPoolDod<HelloWorldMessageComponent>)componentPools[0];
        var componentId = entity.GetComponentId(ComponentType.HELLO_WORLD_COMPONENT);

        dataComponentPool.ReturnObjectWithId(componentId);
    }
}


//public struct TestEntity : IEntity
//{
//    public int Id { get; set; }
//    public int[] components;

//    public TestEntity()
//    {
//        Id = 0;
//        components = new int[6]; // 6 is the number of component types we have
//        for (int i = 0; i < components.Length; i++)
//        {
//            components[i] = -1;
//        }
//    }

//    public void SetComponent(int componentType, int componentId)
//    {
//        components[componentType] = componentId;
//    }

//    public int GetComponentId(int componentType)
//    {
//        return components[componentType];
//    }
//}
