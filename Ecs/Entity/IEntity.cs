namespace TodoApp
{
    /*
    * * An Entity in ECS is only an ID. We can double that ID as an index of the entity's location
    * * in the array which holds all the entities.
    */
    public interface IEntity
    {
        public int Index { get; set; }
    }
}