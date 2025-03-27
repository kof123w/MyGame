using System;
using System.IO;
using System.Collections;
using Mono.Cecil;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MyGame
{
    //Use the basic assets-bundle solution (Async and Sync) 
    public class Resourcer : IResource
    {
        public void UnloadUnusedAssets()
        {
#if UNITY_LOCAL_SCRIPT 
            Resources.UnloadUnusedAssets();
#else
#endif
        }

        public Object LoadResourceAsset(string path, Type type)
        {
            if (type == null)
            {
                Debug.LogError(string.Format("ERROR: Asset type {0} not Exist!", path));
                return null;
            }

            return Resources.Load(path, type);
        }

        public IEnumerator LoadResourceGameObjectAsync(string path, Transform parent, Action<GameObject> onLoaded,AsyncTiming asyncTiming)
        {
#if UNITY_LOCAL_SCRIPT
            //开始异步加载
            ResourceRequest request = Resources.LoadAsync<GameObject>(path); 
            yield return request;
            // 获取加载的资源
            GameObject loadedObject = request.asset as GameObject;
            if (loadedObject != null)
            {
                if (onLoaded != null)
                { 
                    var go = Object.Instantiate(loadedObject, parent, false);
                    onLoaded(go);
                } 
                asyncTiming.LoadFinish();
            }
            else
            {
                Debug.LogError($"Asset loading failed! {path}");
            }
#else
            return null;
#endif
        }
    }
}