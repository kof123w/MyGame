using System;
using System.Threading;
using BEPUphysics.Entities;
using UnityEngine;
using FixedMath;
using Object = UnityEngine.Object;

namespace MyGame
{
    //场景资源对象，包括玩家
    public class Fix64Shape : IMemoryPool
    {
        #region 物理更新
        private CancellationTokenSource fixedUpdateToken; 
        protected Vector3 offsetCenter = Vector3.zero;
        protected float mass = 1;
        private Entity entityShape; 
        protected Collider entityCollider;  //unity的碰撞器直接用到这里来

        protected void SetEntityCollider(Collider entityColliderParam)
        {
            entityCollider = entityColliderParam;
        }

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
        
        
        
        #endregion

        public void OnDestroy()
        {
            throw new NotImplementedException();
        }
    }
}