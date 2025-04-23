using System;
using System.Threading;
using BEPUphysics.Entities;
using UnityEngine;
using FixedMath;
using Object = UnityEngine.Object;

namespace MyGame
{
    //场景资源对象，包括玩家
    public class AssetShape : IMemoryPool
    {
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

        /*public virtual void Start()
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
        }*/
        
        #endregion


        public void Clear()
        {
            
        }
    }
}