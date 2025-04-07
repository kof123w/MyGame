using System;
using Config;
using Cysharp.Threading.Tasks;
using Object = UnityEngine.Object;

namespace MyGame
{
    public class BaseSubScene : AssetObject
    {
        private SceneLoadResourceConfig sceneConfig;
        private SceneType sceneType;
        
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
            await UnloadResource();
        } 
       

        public SceneType GetSceneType()
        {
            return sceneType;
        } 
    }
}