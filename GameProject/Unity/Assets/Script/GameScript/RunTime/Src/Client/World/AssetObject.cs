using System;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Object = UnityEngine.Object;

namespace MyGame
{
    //场景资源对象，包括玩家
    public class AssetObject : IMemoryPool
    {
        protected bool isLoaded = false; 
        protected GameObject gameObject;
        protected Transform trans;
        protected CancellationTokenSource cts = null;
        protected  IProgress<float> tracker = null;   //跟着进度用
        public GameObject AssetObj { get { return gameObject; } }
        
        public bool IsLoaded { get { return isLoaded; } }

        /// <summary>
        /// 加载这个资源
        /// </summary>
        protected async UniTask<Object> LoadAsset(string resourcePath)
        {
            if (cts == null)
            {
                cts = new CancellationTokenSource();
            }
            else
            {
                cts.Cancel();
                cts.Dispose();
                cts = new CancellationTokenSource();
            }

            var obj = await ResourcerDecorator.Instance.LoadResourceAsync(resourcePath,cts.Token,tracker);
             gameObject = (GameObject)Object.Instantiate(obj);
             if (gameObject != null)
             {
                 trans = gameObject.transform;
                 trans.position = Vector3.zero;
             }

             isLoaded = true;
             return obj;
        }

        /// <summary>
        /// 卸载这个资源
        /// </summary>
        protected async UniTask UnloadResource()
        {
            if (!isLoaded)
            {
                cts.Cancel();
                cts.Dispose();
                cts = null;
                return;
            }

            await UniTask.SwitchToMainThread();
            if (gameObject != null)
            {
                Object.Destroy(gameObject);
            }

            isLoaded = false;
            cts.Cancel();
            cts.Dispose();
            cts = null;
        }

        public void SetTracker(IProgress<float> trackerParam)
        {
            this.tracker = trackerParam;
        }
    }
}