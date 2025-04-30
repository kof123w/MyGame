using System;
using System.Threading;
using BEPUphysics.Entities;
using UnityEngine;
using FixedMath;
using FixMath.NET;
using Object = UnityEngine.Object;

namespace MyGame
{
    //场景资源对象，包括玩家
    public class Fix64Shape : IMemoryPool
    {
        #region 物理更新
        private Entity entityShape;   
        protected Fix64 Mass = 1.5M;

        public Entity EntityShape {
            get
            {
                if (entityShape != null)
                {
                    return entityShape;
                }

                entityShape = CreateEntityShape();
                InitEntityShapeParam();
                return entityShape;
            }
        }

        public void SetPosition(FPVector3 position)
        {
            EntityShape.Position = position;
        }

        public virtual void SyncWorld()
        {
            
        }

        protected virtual void InitEntityShapeParam()
        {
            
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