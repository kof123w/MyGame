using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.Entities;
using FixMath.NET; 

namespace MyGame
{
    public class Fix64RoleBody : Fix64Shape
    {
        protected Fix64 Length = 1;
        protected Fix64 Radius = 0.5f;
        protected Fix64 LossyScaleX = 1.0M;
        protected Fix64 LossyScaleY = 1.0M;
        protected Fix64 LossyScaleZ = 1.0M;
        
        
        protected override Entity CreateEntityShape()
        {
            var entity = new Entity(new CapsuleShape(Length * LossyScaleY, Radius * LossyScaleX), Mass);
            entity.Tag = PhysicsTag.PlayerTag;
            FrameContext.Context.AddEntityToWorld(entity);
            return entity; 
        }

        public new void OnDestroy()
        {
            FrameContext.Context.RemoveEntityFromWorld(EntityShape);
        }
    }
}