using Cysharp.Threading.Tasks;
using MyGame;
using SingleTool;

namespace Config
{
    public class ResourceConfigManager : Singleton<ResourceConfigManager>
    {
        private ResDictionary<int, SceneLoadResourceConfig> sceneLoadResourceDict = new(new SceneLoadResourceConfigBinCache());
        private ResDictionary<int,RoleResourceConfig> roleResourceConfigDict = new(new RoleResourceConfigBinCache());
        
        public ResourceConfigManager()
        {
            sceneLoadResourceDict.Init(t=>t.ID);
            roleResourceConfigDict.Init(t=>t.ID);
        }

        public async UniTask<SceneLoadResourceConfig> GetSceneResourceConfig(int sceneId)
        { 
             return await sceneLoadResourceDict.TryGetVal(sceneId);
        }

        public async UniTask<RoleResourceConfig> GetRoleResourceConfig(int roleId)
        {
            return await roleResourceConfigDict.TryGetVal(roleId);
        }
    }
}