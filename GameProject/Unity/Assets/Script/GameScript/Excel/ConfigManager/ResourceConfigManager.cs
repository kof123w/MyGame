using MyGame;

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

        public SceneLoadResourceConfig GetSceneResourceConfig(int sceneId)
        { 
            return sceneLoadResourceDict.TryGetVal(sceneId);
        }

        public RoleResourceConfig GetRoleResourceConfig(int roleId)
        {
            return roleResourceConfigDict.TryGetVal(roleId);
        }
    }
}