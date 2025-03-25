using System;
using UnityEngine;

namespace MyGame
{
    public enum  ResourceLoadMethod
    {
        Local,
        Cdn,
    }

    public class ResourceLoader : UnitySingleton<ResourceLoader>
    {
        private ResourceLoadMethod _mLoaderResourceType = ResourceLoadMethod.Local;
        public Resourcer Resourcer { get; private set; }

        public override void Awake()
        {
            base.Awake();
            if (Resourcer == null)
            {
                Resourcer = new Resourcer();
            } 
        }

        public void SetLoaderResourceType(int loaderResourceType)
        {
            _mLoaderResourceType = (ResourceLoadMethod)loaderResourceType;
        }

        public ResourceLoadMethod GetLoaderResourceType()
        {
            return _mLoaderResourceType;
        }

        public AsyncTiming LoadUIResource(Transform parent, Action<GameObject> callback, string resourceName)
        {
            AsyncTiming asyncTiming = Pool.Malloc<AsyncTiming>();
            asyncTiming.SetAssetBundleCreateRequest(null);
            string loadPath = _mLoaderResourceType == ResourceLoadMethod.Local
                ? $"Assets/Res/UIPrefabs/{resourceName}"
                : $"Assets/StreamingAssets/UIAb/{resourceName}"; 
            
            StartCoroutine(Resourcer.LoadResourceAssetAsync(loadPath, parent, callback,asyncTiming));
            return asyncTiming;
        }

        public void CancelLoading(AsyncTiming asyncTiming)
        {
            if (asyncTiming == null)
            {
                DLogger.Error("AsyncTiming is null,can't cancel loading.....");
                return;
            }
            var cte = asyncTiming.GetCoroutine();
            if (cte != null)
            {
                StopCoroutine(cte);
            } 
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