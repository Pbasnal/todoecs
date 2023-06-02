using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
       
        ref var entity = ref archetype.BuildEntity();

        archetype.StartSystems(tknSource.Token);
    }
}
