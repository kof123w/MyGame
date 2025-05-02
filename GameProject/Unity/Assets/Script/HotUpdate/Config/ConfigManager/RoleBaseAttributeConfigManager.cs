using Cysharp.Threading.Tasks;
using SingleTool;

namespace Config
{
    public class RoleBaseAttributeConfigManager : Singleton<RoleBaseAttributeConfigManager>
    {
        private ResDictionary<int, RoleBaseAttributeConfig> roleBaseAttributeConfigDict =
            new ResDictionary<int, RoleBaseAttributeConfig>(new RoleBaseAttributeConfigBinCache());

        public RoleBaseAttributeConfigManager()
        {
            roleBaseAttributeConfigDict.Init(t=>t.ID);
        }

        public async UniTask<RoleBaseAttributeConfig> GetRoleBaseAttributeConfig(int id)
        {  
            return await roleBaseAttributeConfigDict.TryGetVal(id);
        }
    }
}