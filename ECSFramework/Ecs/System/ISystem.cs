namespace ECSFramework;

/*
* * A system is a unit of logic that performs some operations on one or multiple types 
* * of components. 
* * Since, a system works on specific type of components and components are stored in
* * contigous arrays, systems experience very less cpu cache misses.
* * 
* * There are other benefits of constructing systems this way.
* * 1. Every system operates on specified components, which means we can parallelize 
* *    most  systems
* * 2. We get architectural benefit as well because systems now have a defined scope
* *    and they naturally follow the Single Responsibility Principle.
*/
public interface ISystem<E>
    where E : struct, IEntity
{
    public string Name { get; }
    public void Execute(AnEntityArchetype<E> entityArchetype, CancellationToken token);
    public void ExecuteBatch(AnEntityArchetype<E> entityArchetype, int start, int end, CancellationToken token);
}