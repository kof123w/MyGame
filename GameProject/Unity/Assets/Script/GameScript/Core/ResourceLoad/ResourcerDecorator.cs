using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MyGame
{
    public enum ResourceLoadMethod
    {
        Local,
        Cdn,
    }

    public class ResourcerDecorator : Singleton<ResourcerDecorator>
    {
        private ResourceLoadMethod loaderResourceType = ResourceLoadMethod.Local;
        private Resourcer Resourcer { get; set; }

        public void Init()
        {
            Resourcer = new Resourcer();
        }

        public void SetLoaderResourceType(int loaderResourceTypeParam)
        {
            this.loaderResourceType = (ResourceLoadMethod)loaderResourceTypeParam;
        }

        public ResourceLoadMethod GetLoaderResourceType()
        {
            return loaderResourceType;
        }

        public async UniTask<Object> LoadUIResourceAsync(string resourceName,CancellationToken token)
        {
#if UNITY_LOCAL_SCRIPT  
            string path = $"UIPrefabs/{resourceName}";
            return await Resourcer.LoadAsync(path,token);
#else
            return null;
#endif
        }

        public async UniTask<Object> LoadResourceAsync(string path,CancellationToken token,IProgress<float> progress = null)
        {
#if UNITY_LOCAL_SCRIPT  
            return await Resourcer.LoadAsync(path,token,progress);
#else
           return null;
#endif
        } 

        //直接销毁
        public void DestroyUIResource(GameObject gObj, Action callback = null)
        {
            Object.DestroyImmediate(gObj);
            if (callback != null)
            {
                callback?.Invoke();
            }
        }
    }
}