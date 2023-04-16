namespace TodoApp
{
    /*
    * * A component in ECS is an object which holds data. We store components in an array.
    * * This is a very important piece of ECS to understand. Component should be small and 
    * * clean. Component should contain data which is processed together. This helps in 
    * * reducing size of the component and systems can be written for a single responsibility.
    * * 
    * * We should use the SQL Data Normalisation rules to create the components.
    * *
    */
    public interface IComponent
    {
        // Not exactly needed. I'm was just trying a few things.
        public int ComponentTypeId();
        public bool IsSet { get; set; }
    }
}
