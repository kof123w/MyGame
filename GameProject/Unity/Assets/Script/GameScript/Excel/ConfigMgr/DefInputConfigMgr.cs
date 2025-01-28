using UnityEngine;

namespace Config
{
    public class DefInputConfigMgr : Singleton<DefInputConfig>
    {
        private ResDictionary<int, DefInputConfig> m_defInputConfigDict = new ResDictionary<int, DefInputConfig>();

        public DefInputConfigMgr()
        {
            m_defInputConfigDict.Init(t=>t.ID);
        }
    }
}
