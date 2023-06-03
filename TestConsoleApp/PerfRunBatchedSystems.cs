using BenchmarkDotNet.Attributes;

using ECSFramework;

namespace TestConsoleApp;

[MemoryDiagnoser]
public class PerfRunBatchedSystems
{
    private int initialSizeOfArchetype = 100;
    private int countOfNumbersToSort = 10000;
    
    private int numberOfEntities = 1000;
    private int batchSize = 125;

    [Benchmark]
    public void RunSyncEcs()
    {
        var tknSource = new CancellationTokenSource();
        var archetype = new TestArchetype(initialSizeOfArchetype);

        for (int i = 0; i < numberOfEntities; i++)
        {
            archetype.BuildEntity(countOfNumbersToSort);
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
            archetype.BuildEntity(countOfNumbersToSort);
        }

        archetype.StartSystemsAsync(batchSize, tknSource.Token).Wait();
    }
}
