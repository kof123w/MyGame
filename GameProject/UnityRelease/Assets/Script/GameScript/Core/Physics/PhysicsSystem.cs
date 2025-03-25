
using System;
using BEPUphysics;
using FixedMath;
using FixMath.NET;
using UnityEngine;

namespace MyGame
{
    public class PhysicsSystem : UnitySingleton<PhysicsSystem>
    {
        private BEPUphysicsSpace _mPhysicsBepUphysicsSpace;
        public override void Awake()
        {
            base.Awake();
            if (Instance != null)
            {
                return;
            }
        }

        public void Initialize()
        {
            DLogger.Log("==============Initializing Physics System===================");
            //创建物理世界，设置重力加速度
            _mPhysicsBepUphysicsSpace = new BEPUphysicsSpace();
            _mPhysicsBepUphysicsSpace.ForceUpdater.Gravity = new FPVector3(0, (Fix64)(-9.81m), 0);
            _mPhysicsBepUphysicsSpace.TimeStepSettings.TimeStepDuration = 1 / 60m;
            
            //关掉物理系统
            Physics.autoSyncTransforms = false;  //射线检测关闭
            Physics.autoSimulation = false;
        }

        private void Update()
        {
            if (_mPhysicsBepUphysicsSpace != null)
            {
                _mPhysicsBepUphysicsSpace.Update();
            }
        }
    }
}
