 

using System.Collections.Generic;

namespace MyGame
{
    public abstract class Entity : IMemoryPool
    { 
        public long BaseID { get; private set; }
        private List<long> m_Components = new List<long>();
        public void SetId(long id)
        {
            BaseID = id;
            m_Components.Clear();
        } 

        public bool AddComponent(long id)
        {
            if (!IsContainComponent(id))
            {
                m_Components.Add(id);
                return true;
            }
            return false;
        }

        public bool RemoveComponent(long id)
        {
            int index;
            GetComponentIndex(id, out index);
            if (index >= 0)
            {
                m_Components.RemoveAt(index);
                return true;
            }
            
            return false;
        }

        private void GetComponentIndex(long id, out int index)
        {
            index = -1;
            for (int i = 0; i < m_Components.Count; i++)
            {
                if (m_Components[i] == id)
                {
                    index = i;
                    break;
                }
            }
        }

        private bool IsContainComponent(long id)
        {
            for (int i = 0; i < m_Components.Count; i++) {
                if (m_Components[i] == id)
                {
                    return true;
                }
            }

            return false;
        }

        public List<long> GetComponents()
        {
            return m_Components;
        }

        public void ClearComponents()
        {
            if (m_Components != null)
            { 
                m_Components.Clear();
                m_Components.Capacity = 1;
            }
        }
    }
}
