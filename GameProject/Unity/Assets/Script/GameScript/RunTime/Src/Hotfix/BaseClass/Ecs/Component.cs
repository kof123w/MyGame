 
namespace MyGame
{
    public abstract class ComponentData
    {
        protected bool m_enable = false;
        protected int m_activeCnt = 0;
        public bool Enable
        {
            get { return m_enable; }
            set
            {
                if (m_enable != value)
                {
                    if (value)
                    {
                        GameWorld.Instance.Push(GameWorldConst.GameWorldEnableEventId,EntityId,ComponentId);
                        if (m_activeCnt==0)
                        {
                            GameWorld.Instance.Push(GameWorldConst.GameWorldStartEventId,EntityId,ComponentId);
                            m_activeCnt++;
                        }
                    }
                    else
                    {
                        GameWorld.Instance.Push(GameWorldConst.GameWorldDisableEventId,EntityId,ComponentId);
                    }
                }
            }
        }

        public long EntityId { get; set; }
        public long ComponentId { get; set; }
        public long SystemId { get; set; }
    }
}
