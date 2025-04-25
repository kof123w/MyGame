using BEPUphysics.Entities;
using FixMath.NET;
using MyGame;
using UnityEngine;
 

namespace FixedPhysicsComponent
{  
    public class Fix64Box : Fix64Shape
    {
        BoxCollider boxCollider = null;  
        protected override Entity CreateEntityShape()
        {
            boxCollider = (BoxCollider)entityCollider;
            Fix64 width = boxCollider.size.x;
            Fix64 height = boxCollider.size.y;
            Fix64 lenght = boxCollider.size.z; 
            offsetCenter = boxCollider.center;
            Vector3 unityPos = boxCollider.transform.position + boxCollider.center;
            var entityShopShape = new BEPUphysics.Entities.Prefabs.Box(MathConvertor.Vector3ToFix3(ref unityPos),width,height,lenght,mass); 
            var physicsSpace = FrameContext.Context.GetSpace();
            physicsSpace.Add(entityShopShape);   
            return entityShopShape;
        }

        public new void OnDestroy()
        {
            var physicsSpace = FrameContext.Context.GetSpace();
            physicsSpace.Add(EntityShape);   
        }
    }
}