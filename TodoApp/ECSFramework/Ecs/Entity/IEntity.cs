using ObjectPoolSystem;

namespace ECSFramework;

/*
* * An Entity in ECS is only an ID. We can double that ID as an index of the entity's location
* * in the array which holds all the entities.
*/
public interface IEntity : IValueObject
{
    void MapComponentToEntity<T>(T component) where T : struct, IComponent;

    void MapComponentToEntity(int componentType, int componentId);

    int GetComponentId(int componentType);
}