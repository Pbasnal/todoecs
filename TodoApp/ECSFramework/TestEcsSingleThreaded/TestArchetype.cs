namespace ECSFramework;

public class TestArchetype : AnEntityArchetype<EntityInitializerComponent, EntityFinalizerComponent>
{
    public TestArchetype(int initialNumberOfEntities) : base(initialNumberOfEntities)
    { }

    internal override IList<IComponentPool> GetAllComponentPools(int initialNumberOfEntities)
    {
        return new List<IComponentPool> {
            new ComponentPoolDod<HelloWorldMessageComponent>(initialNumberOfEntities)
        };
    }

    internal override void AddComponentsToEntity(ref EcsEntity entity, ref EntityInitializerComponent requestData)
    {
        var dataComponentPool = GetComponentPool<HelloWorldMessageComponent>();
        ref var helloWorldMessageComponent = ref dataComponentPool.GetFreeObject();
        helloWorldMessageComponent.Init(requestData.LengthToArrayToSort);
        helloWorldMessageComponent.EntityId = entity.Id;
        entity.MapComponentToEntity(helloWorldMessageComponent);
    }

    internal override List<ISystem> GetSystems()
    {
        return new List<ISystem>{
            new SortSetterSystem(),
            new SortSystem()
        };
    }

    internal override void FreeComponentsOfEntity(ref EcsEntity entity)
    {
        var dataComponentPool = GetComponentPool<HelloWorldMessageComponent>();
        var componentTypeId = ComponentType.GetComponentTypeId(typeof(HelloWorldMessageComponent));
        try
        {
            var componentId = entity.GetComponentId(componentTypeId);
            dataComponentPool.ReturnObjectWithId(componentId);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    internal override void AddComponentToEntity<C>(ref EcsEntity entity, C component)
    {
        var dataComponentPool = GetComponentPool<C>();

        var componentTypeId = ComponentType.GetComponentTypeId(component.GetType());
        int componentId = dataComponentPool.GetIndexOfFreeObject();

        entity.MapComponentToEntity(componentTypeId, componentId);
    }
}
