using BenchmarkDotNet.Attributes;

using ECSFramework;

namespace TestConsoleApp;

[MemoryDiagnoser]
public class PerfRunEcs
{
    public void RunSyncEcs()
    {
        var tknSource = new CancellationTokenSource();
        var archetype = new TestArchetype(100);

        var inputComponent = new EntityInitializerComponent();
        inputComponent.LengthToArrayToSort = 10;

        ref var entity = ref archetype.CreateEntity(ref inputComponent);

        archetype.StartSystems(tknSource.Token);
    }
}
