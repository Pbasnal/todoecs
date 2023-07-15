using BenchmarkDotNet.Attributes;

using ECSFramework;

namespace TestConsoleApp;

[MemoryDiagnoser]
public class PerfRunBatchedSystems
{
    private int initialSizeOfArchetype = 50;
    private int countOfNumbersToSort = 10000;
    
    private int numberOfEntities = 100;
    private int batchSize = 50;

    [Benchmark]
    public void RunSyncEcs()
    {
        var tknSource = new CancellationTokenSource();
        var archetype = new TestArchetype(initialSizeOfArchetype);

        for (int i = 0; i < numberOfEntities; i++)
        {
            var inputComponent = new EntityInitializerComponent();
            inputComponent.LengthToArrayToSort = countOfNumbersToSort;

            archetype.CreateEntity(ref inputComponent);
        }        

        archetype.StartSystems(tknSource.Token);
    }

    [Benchmark]
    public void RunAsyncEcs()
    {
        var tknSource = new CancellationTokenSource();
        var archetype = new TestArchetype(initialSizeOfArchetype);

        for (int i = 0; i < numberOfEntities; i++)
        {
            var inputComponent = new EntityInitializerComponent();
            inputComponent.LengthToArrayToSort = countOfNumbersToSort;

            archetype.CreateEntity(ref inputComponent);
        }

        archetype.StartSystemsAsync(batchSize, tknSource.Token).Wait();
    }
}
