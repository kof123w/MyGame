using System;
using System.Threading;
using BEPUphysics.Entities;
using Cysharp.Threading.Tasks;
using FixedMath;
using FixMath.NET;
using MyGame;
using UnityEngine;
 

namespace FixedPhysicsComponent
{  
    public class AssetFix64Box : AssetShape
    {
        BoxCollider boxCollider = null;  
        protected override Entity CreateEntityShape()
        {
            boxCollider = trans.GetComponent<BoxCollider>();
            Fix64 width = boxCollider.size.x;
            Fix64 height = boxCollider.size.y;
            Fix64 lenght = boxCollider.size.z; 
            offsetCenter = boxCollider.center;
            Vector3 unityPos = worldTransform.position + boxCollider.center;
            var entityShopShape = new BEPUphysics.Entities.Prefabs.Box(MathConvertor.Vector3ConvertToFpVector3(ref unityPos),width,height,lenght,mass); 
            var physicsSpace = GameWorld.GetPhysicsSpace();
            physicsSpace.Add(entityShopShape);   
            return entityShopShape;
        }
    }
}