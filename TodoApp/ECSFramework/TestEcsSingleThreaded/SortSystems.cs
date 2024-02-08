namespace ECSFramework;

public class SortSetterSystem : ASystemBase<HelloWorldMessageComponent>
{
    public override string Name => "SortSetterSystem";

    private Random random = new Random();

    protected override void ProcessComponent(ref HelloWorldMessageComponent component)
    {
        int j = 0;
        for (; j < component.Nums.Length; j++)
        {
            component.Nums[j] = random.Next(100);
        }

        //Console.WriteLine($"SortSetter: Id: {component.Id} nums: {GetNumsStr(component.Nums)} j: {j}");
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

public class SortSystem : ASystemBase<HelloWorldMessageComponent, EntityFinalizerComponent>
{
    public override string Name => "SortSystem";

    protected override void ProcessComponent(ref HelloWorldMessageComponent helloComponent, 
        ref EntityFinalizerComponent finalComponent)
    {
        Array.Sort(helloComponent.Nums);
        //SortUtils.BubbleSort(component.Nums);
        //Console.WriteLine($"SortSystem: id: {component.Id} nums: {GetNumsStr(component.Nums)}");
        helloComponent.IsSet = true;
        finalComponent.IsSet = true;

        // this is being set by the second system which is an anti pattern.
        // Future work to come up with an existance based approach
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
