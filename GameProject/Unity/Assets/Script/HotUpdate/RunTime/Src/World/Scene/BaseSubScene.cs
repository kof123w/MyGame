using System;
using Config;
using Cysharp.Threading.Tasks;

namespace MyGame
{
    //临时简单处理一下，目前场景比较简单，一个场景只有一张平台
    public class BaseSubScene : VisualShape
    {
        private SceneLoadResourceConfig sceneConfig;
        private SceneType sceneType;

        public async void SetScene(int configId, SceneType st)
        {
            sceneType = st;
            sceneConfig = await ResourceConfigManager.Instance.GetSceneResourceConfig(configId);
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