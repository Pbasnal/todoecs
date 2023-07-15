using BenchmarkDotNet.Running;

using ECSFramework;

using ObjectPoolSystem;

namespace TestConsoleApp;

public class TestConsoleApp
{
    public static void Main()
    {
        PerfRunThreaded();
    }

    public static void PerfRunThreaded()
    {
        BenchmarkRunner.Run<PerfRunBatchedSystems>();

        //int numberOfRuns = 1;
        //int[][] arr = new int[numberOfRuns][];
        //var random = new Random();

        //var perfSystem = new PerfRunBatchedSystems();
        //for (int i = 0; i < numberOfRuns; i++)
        //{
        //    var startTime = DateTime.Now;
        //    perfSystem.RunAsyncEcs();

        //    //arr[i] = new int[10000];
        //    //for (int j = 0; j < arr[i].Length; j++)
        //    //{
        //    //    arr[i][j] = j;
        //    //}

        //    //SortUtils.BubbleSort(arr[i]);

        //    var endTime = DateTime.Now;
        //    Console.WriteLine($"Time taken: {(endTime - startTime).TotalMilliseconds}");
        //}
    }


    public static void PerfRunPool()
    {
        //BenchmarkRunner.Run<ListVsMemory>();
        BenchmarkRunner.Run<PerfRunsOnPool>();
    }
}