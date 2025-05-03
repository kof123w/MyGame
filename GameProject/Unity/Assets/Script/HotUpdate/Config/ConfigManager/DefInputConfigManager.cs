using System.Collections.Generic;
using Cysharp.Threading.Tasks;
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

        public async UniTask<List<DefInputConfig>> GetInputConfigs()
        {
            return await defInputConfigDict.GetCacheList();
        }  
    }
}
