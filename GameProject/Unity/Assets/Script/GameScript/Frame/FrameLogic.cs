using System.Linq;
using BEPUphysics;
using FixedMath;
using FixedMath.Threading;
using SingleTool;
using UnityEngine;

namespace MyGame
{
    public class FrameLogic : Singleton<FrameLogic>
    {
        internal SRandom sRandom;
        private BEPUphysicsSpace bEpUPhysicsSpace;
        
        private FrameInputSample frameInputSample;
        private INetworkService networkService;  //临时使用依赖注入的方法 
        
        //设置一下tick时间 
        private int tick;
        private float tickTime;
        private float curTickTime;
        
        internal void SetTick(int tickParam)
        {
            tick = tickParam;
            tickTime = 1000.0f / tick;
            curTickTime = 0.0f;
        }
        
        private bool IsUpdate;
        //开启帧同步核心逻辑
        public void Start(int randomSeed,int tickParam,INetworkService network)
        {
            sRandom = new SRandom(randomSeed);
            SetTick(tickParam);
            networkService = network;
            //创建物理世界，设置重力加速度，多线程工作设置
            frameInputSample = new FrameInputSample();
            frameInputSample.SubscribeEvent();
            var parallelLooper = new ParallelLooper();
            bEpUPhysicsSpace = new BEPUphysicsSpace(parallelLooper);
            bEpUPhysicsSpace.ForceUpdater.Gravity = new FPVector3(0, -9.81m, 0);
            bEpUPhysicsSpace.TimeStepSettings.TimeStepDuration = Time.fixedDeltaTime;

            //强制多线程确定性模式
            bEpUPhysicsSpace.BroadPhase.AllowMultithreading = false;
            bEpUPhysicsSpace.NarrowPhase.AllowMultithreading = false;
            bEpUPhysicsSpace.Solver.AllowMultithreading = false;
        }
        
        
        public void FixedTick()
        {
            if (!IsUpdate)
            {
                return;
            }
            curTickTime += Time.fixedDeltaTime;
            
            //进行输入采集 
            frameInputSample.InputSample(curTickTime,tickTime); 
            bEpUPhysicsSpace.Update();

            if (curTickTime >= tickTime)
            {
                //打包发送
                CSFrameSample csFrameSample = frameInputSample.PackInput();
                networkService.Send((int)MessageType.CscsframeSample,ProtoHelper.Serialize(csFrameSample));
                
                curTickTime = 0.0f;
            }
        }


        public void CloseFrameSync()
        {
            IsUpdate = false;
            frameInputSample.UnSubscribeEvent();
            
            // 移除所有实体
            foreach (var entity in bEpUPhysicsSpace.Entities.ToArray()) // 使用ToArray避免修改集合
            {
                bEpUPhysicsSpace.Remove(entity);
            }
            
        }
    }
}