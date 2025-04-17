using System;
using System.Threading;
using AssetsLoad;
using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using UnityEngine;
using Cysharp.Threading.Tasks;
using FixedMath;
using FixMath.NET;
using ObjectPool;
using Object = UnityEngine.Object;

namespace MyGame
{
    //场景资源对象，包括玩家
    public class AssetShape : IMemoryPool
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
        public AssetShape()
        {
            worldGameObject = NodePool.MallocEmptyNode();
            worldTransform = worldGameObject.transform;
            worldTransform.SetParent(GameWorld.GetGameWorldTransform());
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
     
        #region 物理更新

        private CancellationTokenSource fixedUpdateToken; 
        protected Vector3 offsetCenter = Vector3.zero;
        protected float mass = 1;
        private Entity entityShape; 

        protected Entity EntityShape {
            get
            {
                if (entityShape != null)
                {
                    return entityShape;
                }

                entityShape = CreateEntityShape();
                return entityShape;
            }
            set => entityShape = value;
        }

        protected virtual Entity CreateEntityShape()
        {
            return null;
        } 

        public virtual void Start()
        {
            entityShape = CreateEntityShape();
            fixedUpdateToken?.Cancel();
            fixedUpdateToken?.Dispose(); 
            fixedUpdateToken = new CancellationTokenSource();
            FixedUpdate(fixedUpdateToken.Token).Forget();
        }
        
        protected virtual async UniTaskVoid FixedUpdate(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate,token); 
                if (worldTransform != null)
                {
                    FPVector3 fpPos = entityShape.Position;  
                    var unityWorldPosition = MathConvertor.FpVector3ConvertToVector3(ref fpPos);
                    worldTransform.position = unityWorldPosition - offsetCenter;
                    var fpRotation = entityShape.Orientation;
                    worldTransform.rotation = MathConvertor.FpQuaternionToQuaternion(ref fpRotation); 
                }
            } 
        }
        
        protected virtual void OnDestroy()
        { 
            fixedUpdateToken?.Cancel();
            fixedUpdateToken?.Dispose();
            fixedUpdateToken = null;
            roleTransform = null;
            gameObject = null; 
            NodePool.FreeEmptyNode(worldGameObject);
            worldTransform = null;
            worldGameObject = null;
            var physicsSpace = GameWorld.GetPhysicsSpace();
            physicsSpace.Remove(entityShape);
        }
        
        #endregion


    }
}