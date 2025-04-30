using BEPUphysics.Entities; 
using UnityEngine;

namespace MyGame
{
    public class Fix64Cylinder : Fix64Shape
    {
        private CapsuleCollider unityCapsule;   
        protected float Height;
        protected float Radius;

        protected override Entity CreateEntityShape()
        {
            //获取 Unity CapsuleCollider参数
            /*unityCapsule = (CapsuleCollider)entityCollider; 
            Radius = unityCapsule.radius;
            Height = unityCapsule.height;
            var pos = entityCollider.transform.position;
            var fpPos = MathConvertor.Vector3ToFix3(ref pos);
            var cylinderBody = new BEPUphysics.Entities.Prefabs.Cylinder(fpPos, Height, Radius, mass); 
            return cylinderBody;*/

            return null;
        }
        
        public new void OnDestroy()
        {
            var physicsSpace = FrameContext.Context.GetSpace();
            physicsSpace.Add(EntityShape);   
        }
    }
}