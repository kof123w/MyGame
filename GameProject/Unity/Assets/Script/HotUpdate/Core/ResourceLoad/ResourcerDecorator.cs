using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DebugTool;
using SingleTool;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AssetsLoad
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
            DLogger.Log("Init Resourcer");
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

        public async UniTask<TextAsset> LoadConfigAssetAsync(string resName)
        { 
            return await Resourcer.LoadConfigAsset(resName);
        }

        public async UniTask<Object> LoadUIResourceAsync(string resourceName,CancellationToken token)
        {
            return await Resourcer.LoadAsync(resourceName,token);
        }

        public async UniTask<Object> LoadResourceAsync(string path,CancellationToken token,IProgress<float> progress = null)
        {
            return await Resourcer.LoadAsync(path,token,progress);
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