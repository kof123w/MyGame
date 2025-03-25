using System.Collections.Generic; 
using MyGame;
namespace Config
{
    public class DefInputConfigMgr : Singleton<DefInputConfigMgr>
    {
        private ResDictionary<int, DefInputConfig> m_defInputConfigDict = new(new DefInputConfigBinCache());

        public DefInputConfigMgr()
        {
            m_defInputConfigDict.Init(t=>t.ID);
        }

        public List<DefInputConfig> GetInputConfigs()
        {
            return m_defInputConfigDict.GetCacheList;
        } 
    }
}
