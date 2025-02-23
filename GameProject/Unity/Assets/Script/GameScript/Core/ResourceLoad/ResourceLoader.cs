using System;
using UnityEngine;

namespace MyGame
{
    public class ResourceLoader : UnitySingleton<ResourceLoader>
    {
        public Resource Resourcer { get; private set; }
        
        
        
        //加载UI用到
        public void LoadUIResource(Type type,Transform parent,Action<GameObject> callback,string resourceName)
        {
            StartCoroutine(Resourcer.LoadResourceAssetAsync($"Assets/StreamingAssets/UI/{resourceName}", parent, callback));
        }
        
        //直接销毁
        public void DestroyUIResource(GameObject gObj, Action callback = null)
        { 
            DestroyImmediate(gObj);
            if (callback != null)
            {
                callback?.Invoke();
            }
        }
    }
}