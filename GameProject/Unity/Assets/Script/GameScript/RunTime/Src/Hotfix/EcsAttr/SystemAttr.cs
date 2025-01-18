using System;

namespace MyGame
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class System : Attribute
    {
        private Type m_systemType;

        public System(Type systemType)
        {
            m_systemType = systemType;
        }

        public Type GetSystemType()
        {
            return m_systemType;
        }
    }
}

