 
using MyGame.Define;

namespace MyGame
{
    public abstract class ComponentData : IMemoryPool
    {
        public volatile ComponentType ComponentType = ComponentType.Data; 
        public long EntityId { get;private set; }
        public long BaseId { get; private set; }

        public volatile bool IsLoaded = false;

        public volatile bool IsFree = false;

        public ComponentType SetComponentType(ComponentType componentType)
        {
            return ComponentType = componentType;
        }

        public bool SetIsFree(bool isFree)
        {
            return IsFree = isFree;
        }

        public bool SetIsLoaded(bool isLoaded)
        {
            return IsLoaded = isLoaded;
        } 

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
