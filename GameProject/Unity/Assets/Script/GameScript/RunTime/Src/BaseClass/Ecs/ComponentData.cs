 
namespace MyGame
{
    public abstract class ComponentData : IMemoryPool
    {
        public long EntityId { get;private set; }
        public long BaseId { get; private set; }

        public void SetId(long id)
        {
            BaseId = id;
        }

        public void SetEntityId(long entityId)
        {
            EntityId = entityId;
        }
    }
}
