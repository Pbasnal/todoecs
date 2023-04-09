namespace TodoApp
{
    public interface ISystem<E> where E : IEntity, new()
    {
        public void Execute(EntityArchetype<E> entityArchetype);
    }
}