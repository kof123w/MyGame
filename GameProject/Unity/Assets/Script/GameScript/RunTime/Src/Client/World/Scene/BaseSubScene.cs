using Config;
using Cysharp.Threading.Tasks;

namespace MyGame
{
    //临时简单处理一下，目前场景比较简单，一个场景只有一张平台
    public class BaseSubScene : AssetFix64Terrain
    {
        private SceneLoadResourceConfig sceneConfig;
        private SceneType sceneType;

        public override void Start()
        {
            mass = 0;
            base.Start(); 
        }

        public void SetScene(int configId, SceneType st)
        {
            sceneType = st;
            sceneConfig = ResourceConfigManager.Instance.GetSceneResourceConfig(configId);
        } 
        
        public virtual async UniTask LoadScene()    
        {
             await LoadAsset(sceneConfig.AssetPath);
        }

        
        public virtual async UniTaskVoid UnloadScene()
        { 
            OnDestroy();
            await UnloadResource(); 
        } 
        
        public SceneType GetSceneType()
        {
            return sceneType;
        } 
    }
}