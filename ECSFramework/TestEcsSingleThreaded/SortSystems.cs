namespace ECSFramework;

public class SortSetterSystem : ISystem<EcsEntity>
{
    public string Name => "SortSetterSystem";

    private Random random = new Random();

    public void Execute(AnEntityArchetype<EcsEntity> entityArchetype, CancellationToken token)
    {
        var componentPool = entityArchetype.GetComponentPool<HelloWorldMessageComponent>(0);
        var span = componentPool.GetActiveObjects();
        ExecuteSystem(span);
        //Console.WriteLine("Finishing sort setter");
    }

    public void ExecuteBatch(AnEntityArchetype<EcsEntity> entityArchetype, int batchSize, CancellationToken token)
    {
        var componentPool = entityArchetype.GetComponentPool<HelloWorldMessageComponent>(0);
        var memory = componentPool.GetActiveObjects();
        if (memory.IsEmpty) return;

        var systemTasks = new List<Task>();
        for (int i = 0; i < memory.Length; i += batchSize)
        {
            var len = memory.Length > i + batchSize ? batchSize : memory.Length - i;
            systemTasks.Add(StartSystem(memory.Slice(i, len)));
        }

        Task.WaitAll(systemTasks.ToArray());
    }

    private async Task StartSystem(Memory<HelloWorldMessageComponent> memory)
    {
        await Task.Factory.StartNew(() => ExecuteSystem(memory));
    }

    private void ExecuteSystem(Memory<HelloWorldMessageComponent> memory)
    {
        var span = memory.Span;
        for (int i = 0; i < span.Length; i++)
        {
            ref var component = ref span[i];
            if (component.IsSet) continue; // this is being set by the second system which is an anti pattern.
                                           // Future work to come up with an existance based approach

            int j = 0;
            for (; j < component.Nums.Length; j++)
            {
                component.Nums[j] = random.Next(100);
            }

            //Console.WriteLine($"SortSetter: Id: {component.Id} nums: {GetNumsStr(component.Nums)} j: {j}");
        }
    }

    private string GetNumsStr(int[] nums)
    {
        string numsStr = nums.Length + " elements";
        //for (int i = 0; i < nums.Length; i++)
        //{
        //    numsStr += nums[i] + " ";
        //}
        return numsStr;
    }
}

public class SortSystem : ISystem<EcsEntity>
{
    public string Name => "SortSystem";

    public void Execute(AnEntityArchetype<EcsEntity> entityArchetype, CancellationToken token)
    {
        ExecuteSystem(entityArchetype, entityArchetype.GetActiveEntities());
        //Console.WriteLine("Finishing sort system");
    }

    public void ExecuteBatch(AnEntityArchetype<EcsEntity> entityArchetype, int batchSize, CancellationToken token)
    {
        var entities = entityArchetype.GetActiveEntities();
        if (entities.IsEmpty) return;

        var systemTasks = new List<Task>();
        for (int i = 0; i < entities.Length; i += batchSize)
        {
            var len = entities.Length > i + batchSize ? batchSize : entities.Length - i;
            systemTasks.Add(StartSystem(entityArchetype, entities.Slice(i, len)));
        }

        Task.WaitAll(systemTasks.ToArray(), token);
    }

    private async Task StartSystem(AnEntityArchetype<EcsEntity> entityArchetype,
        Memory<EcsEntity> memory)
    {
        await Task.Factory.StartNew(() => ExecuteSystem(entityArchetype, memory));
    }


    private void ExecuteSystem(AnEntityArchetype<EcsEntity> entityArchetype,
        Memory<EcsEntity> memory)
    {
        var componentPool = entityArchetype
             .GetComponentPool<HelloWorldMessageComponent>(ComponentType.HELLO_WORLD_COMPONENT);
        var span = memory.Span;
        for (int i = 0; i < span.Length; i++)
        {
            ref var entity = ref span[i];
            var componentId = entity.GetComponentId(ComponentType.HELLO_WORLD_COMPONENT);
            if (componentId == -1) continue;

            ref var component = ref componentPool.GetObjectWithId(componentId);
            if (!component.IsSet)
            {
                Array.Sort(component.Nums);
                //SortUtils.BubbleSort(component.Nums);
                //Console.WriteLine($"SortSystem: id: {component.Id} nums: {GetNumsStr(component.Nums)}");
                component.IsSet = true;
            }
            entityArchetype.FreeEntity(entity.Id);
            // this is being set by the second system which is an anti pattern.
            // Future work to come up with an existance based approach
        }
    }


    private string GetNumsStr(int[] nums)
    {
        string numsStr = nums.Length + " elements";
        //for (int i = 0; i < nums.Length; i++)
        //{
        //    numsStr += nums[i] + " ";
        //}
        return numsStr;
    }
}

public class SortUtils
{
    public static void BubbleSort(int[] arr)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            for (int j = i; j < arr.Length; j++)
            {
                if (arr[i] < arr[j])
                {
                    var tmp = arr[i];
                    arr[i] = arr[j];
                    arr[j] = tmp;
                }
            }
        }
    }
}
