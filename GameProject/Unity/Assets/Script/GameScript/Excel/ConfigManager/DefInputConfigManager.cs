using System.Collections.Generic; 
using MyGame;
namespace Config
{
    public class DefInputConfigManager : Singleton<DefInputConfigManager>
    {
        private ResDictionary<int, DefInputConfig> m_defInputConfigDict = new(new DefInputConfigBinCache());

        public DefInputConfigManager()
        {
            m_defInputConfigDict.Init(t=>t.ID);
        }

        public List<DefInputConfig> GetInputConfigs()
        {
            return m_defInputConfigDict.GetCacheList;
        }  
    }
}
