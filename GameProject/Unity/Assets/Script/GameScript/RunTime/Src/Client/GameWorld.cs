using System;
using BEPUphysics;
using FixedMath;
using FixMath.NET;
using UnityEngine;
 
namespace MyGame
{
    public sealed class GameWorld : Singleton<GameWorld>
    { 
        private BEPUphysicsSpace bEPUphysicsSpace;
        
        public void Init()
        {
            DLogger.Log("==============>Init world physics system!");
            //关掉物理系统
            Physics.autoSyncTransforms = false;  //射线检测关闭
            Physics.autoSimulation = false;
            
            //创建物理世界，设置重力加速度
            bEPUphysicsSpace = new BEPUphysicsSpace();
            bEPUphysicsSpace.ForceUpdater.Gravity = new FPVector3(0, (Fix64)(-9.81m), 0);
            bEPUphysicsSpace.TimeStepSettings.TimeStepDuration = 1 / 60m;
        }
        
        public void Update()
        { 
            if (bEPUphysicsSpace != null)
            {
                bEPUphysicsSpace.Update();
            }
        }

        public void LateUpdate()
        {
            
        }  
    }  
}
