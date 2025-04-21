using BEPUphysics.Entities;

namespace MyGame
{
    //同步下可见的物体
    public class FixShape
    {
        #region 物理更新  
        protected float Mass = 1;
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
        #endregion
    }
}