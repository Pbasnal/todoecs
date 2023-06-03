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

    public override ComponentPoolDod<TC> GetComponentPool<TC>(int componentType)
    {
        return componentType switch
        {
            ComponentType.HELLO_WORLD_COMPONENT => (ComponentPoolDod<TC>)componentPools[0],
            _ => (ComponentPoolDod<TC>)componentPools[0],
        };
    }

    internal override void AddComponentsToEntity(ref EcsEntity entity, int requestData)
    {
        var dataComponentPool = (ComponentPoolDod<HelloWorldMessageComponent>)componentPools[0];
        ref var helloWorldMessageComponent = ref dataComponentPool.GetFreeObject();
        helloWorldMessageComponent.Init();
        helloWorldMessageComponent.Nums = new int[requestData];
        entity.SetComponent(helloWorldMessageComponent.ComponentTypeId(), helloWorldMessageComponent.Id);
    }

    protected override ISystem<EcsEntity>[] GetSystems()
    {
        return new ISystem<EcsEntity>[] {
            new SortSetterSystem(),
            new SortSystem()
        };
    }

    protected override void FreeComponentsOfEntity(ref EcsEntity entity)
    {
        var dataComponentPool = (ComponentPoolDod<HelloWorldMessageComponent>)componentPools[0];
        var componentId = entity.GetComponentId(ComponentType.HELLO_WORLD_COMPONENT);

        dataComponentPool.ReturnObjectWithId(componentId);
    }
}
