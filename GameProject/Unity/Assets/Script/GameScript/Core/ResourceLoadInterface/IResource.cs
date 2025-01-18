using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MyGame
{
    public interface IResource
    {
        void UnloadUnusedAssets();
        
        void DestroyObject(Object go);
        
        void DestroyObject(Object go, float delayTime);
        
        GameObject AllocGameObject(string resPath, Transform parent);
        
        IEnumerator AllocGameObjectAsync(string resPath, Action<GameObject> onLoaded, Transform parent);

        Object LoadResourceAsset(string path, Type type);

        IEnumerator LoadResourceAssetAsync(string path, Transform parent, Type type, Action<GameObject> onLoaded);
    }
}