using System;
using System.Threading;
using AssetsLoad;
using UnityEngine;
using Cysharp.Threading.Tasks;
using ObjectPool;
using Object = UnityEngine.Object;

namespace MyGame
{
    //场景资源对象，包括玩家
    public class VisualShape : IMemoryPool
    {
        private bool isLoaded = false;
        protected GameObject gameObject;
        protected Transform trans;
        private CancellationTokenSource cts = null;
        private IProgress<float> tracker = null;   //跟着进度用  
        protected GameObject worldGameObject = null;
        protected Transform worldTransform = null;
        protected Transform roleTransform = null;
        public bool IsLoaded => isLoaded; 
        public VisualShape()
        {
            worldGameObject = NodePool.MallocEmptyNode();
            worldTransform = worldGameObject.transform;
            worldTransform.SetParent(GameWorld.GetGameWorldTransform());
        }

        public T[] GetComponentByNodeName<T>(string nodeName)
        {
            var findRoot = trans.Find(nodeName);
            T[] result = null;
            if (findRoot != null)
            {
                result = findRoot.GetComponentsInChildren<T>();
            } 
            return result;
        }

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
             trans.SetParent(worldTransform); 
             trans.localPosition = Vector3.zero;
             isLoaded = true;
             return obj;
        }
        
        public void SetGameObjectName(string name)
        {
            worldGameObject.name = name;
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

        public T GetComponent<T>()
        {
           return trans.GetComponent<T>();
        }

        public T GetComponentInChild<T>()
        {
            return trans.GetComponentInChildren<T>();
        }

        public void OnDestroy()
        {
            
        }
    }
}