using System.Collections.Generic;  
using SingleTool;

namespace Config
{
    public class DefInputConfigManager : Singleton<DefInputConfigManager>
    {
        private ResDictionary<int, DefInputConfig> defInputConfigDict = new(new DefInputConfigBinCache());

        public DefInputConfigManager()
        {
            defInputConfigDict.Init(t=>t.ID);
        }

        public List<DefInputConfig> GetInputConfigs()
        {
            return defInputConfigDict.GetCacheList;
        }  
    }
}
