using System.Threading;
using BEPUphysics.Entities;
using UnityEngine;

namespace MyGame
{
    public class Fix64Fix64Capsule : Fix64Shape
    { 
        private CapsuleCollider unityCapsule;   
        protected float Height;
        protected float Radius;

        protected override Entity CreateEntityShape()
        {/*
            unityCapsule = trans.GetComponent<CapsuleCollider>();
            //获取 Unity CapsuleCollider参数
            Radius = unityCapsule.radius;
            Height = unityCapsule.height;
            Vector3 center = unityCapsule.center;
            int dir = unityCapsule.direction;

            //计算圆柱部分长度
            float cyLinerLength = Mathf.Max(Height - 2 * Radius, 0);

            //确定方向轴
            Vector3 axis = dir switch
            {
                0 => Vector3.right,
                1 => Vector3.up,
                2 => Vector3.forward,
                _ => Vector3.up
            };

            //计算局部坐标下的start 和 end
            Vector3 localStart = center - axis * (cyLinerLength * 0.5f);
            Vector3 localEnd = center + axis * (cyLinerLength * 0.5f);

            //转换成世界坐标
            Vector3 worldStart = unityCapsule.transform.TransformPoint(localStart);
            Vector3 worldEnd = unityCapsule.transform.TransformPoint(localEnd);

            //装换成BEPphics的坐标
            var bePuStart = MathConvertor.Vector3ConvertToFpVector3(ref worldStart);
            var bePuEnd = MathConvertor.Vector3ConvertToFpVector3(ref worldEnd);
            var eCapsule = new BEPUphysics.Entities.Prefabs.Capsule(bePuStart, bePuEnd, Radius, mass);

            /*var physicsSpace = GameWorld.GetPhysicsSpace();
            physicsSpace.Add(eCapsule); #1#
            return eCapsule;*/

            return null;
        }
    }
}