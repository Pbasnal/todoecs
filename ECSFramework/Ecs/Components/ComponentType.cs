namespace ECSFramework;

public class ComponentType
{
    //public const int QUICK_VIEW_REQUEST = 0;
    //public const int QUICK_VIEW_DATA = 1;
    //public const int TASK_ESTIMATES = 2;
    //public const int QUICK_VIEW_TASK_TITLES = 3;
    //public const int QUICK_VIEW_TASK_DATES = 4;
    //public const int QUICK_VIEW_REMINDER = 5;
    //public const int QUICK_VIEW_RESPONSE = 6;
    //public const int ENTITY_INITIALIZER_COMPONENT = 7;
    //public const int HELLO_WORLD_COMPONENT = 8;
    //public const int ENTITY_FINALIZER_COMPONENT = 9;

    public static int Length => componentTypes.Count;

    private static IList<Type> componentTypes;

    static ComponentType()
    {
        componentTypes = new List<Type>()
        {
            typeof(EntityInitializerComponent),
            typeof(HelloWorldMessageComponent),
            typeof(EntityFinalizerComponent)            
        };
    }

    public static int GetComponentTypeId(Type componentType)
    {
        if (componentTypes is null || !componentTypes.Contains(componentType)) return -1;

        return componentTypes.IndexOf(componentType);
    }

    public static int GetComponentTypeId<T>()
    {
        var typeOfComponent = typeof(T);
        if (componentTypes is null || !componentTypes.Contains(typeOfComponent)) return -1;

        return componentTypes.IndexOf(typeOfComponent);
    }
}
