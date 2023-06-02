using BenchmarkDotNet.Running;

using ECSFramework;

using ObjectPoolSystem;

namespace todorest;

public class TestConsoleApp
{
    public static void Main()
    {
        PerfRunEcsFramework();
    }

    public static void PerfRunEcsFramework()
    {
        var tknSource = new CancellationTokenSource();
        var archetype = new TestArchetype(100);

        for (int i = 0; i < 10; i++)
        {
            archetype.BuildEntity();
        }        

        var systemsTask = archetype.StartSystemsAsync(tknSource.Token);       

        //archetype.StartSystems(tknSource.Token).Wait();
        systemsTask.Wait(); 
    }
    

    public static void PerfRunPool()
    {
        //BenchmarkRunner.Run<ListVsMemory>();
        BenchmarkRunner.Run<PerfRunsOnPool>();
    }
}