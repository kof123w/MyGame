using BEPUphysics.Entities; 
using UnityEngine;

namespace MyGame
{
    public class AssetFix64Cylinder : AssetShape
    {
        private CapsuleCollider unityCapsule;   
        protected float Height;
        protected float Radius;

        protected override Entity CreateEntityShape()
        {
            /*unityCapsule = trans.GetComponent<CapsuleCollider>();
            //获取 Unity CapsuleCollider参数
            Radius = unityCapsule.radius;
            Height = unityCapsule.height;
            var pos = trans.position;
            var fpPos = MathConvertor.Vector3ConvertToFpVector3(ref pos);
            var cylinderBody = new BEPUphysics.Entities.Prefabs.Cylinder(fpPos, Height, Radius, mass); 
            return cylinderBody;*/

            return null;
        }
    }
}