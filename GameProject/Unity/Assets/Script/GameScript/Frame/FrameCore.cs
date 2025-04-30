using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using BEPUphysics;
using DebugTool;
using EventSystem;
using FixedMath;
using FixedMath.Threading;
using FixMath.NET;
using NUnit.Framework;
using SingleTool;
using UnityEngine;

namespace MyGame
{
    public class FrameCore
    {
        internal SRandom sRandom;
        
        internal FrameInputSample frameInputSample;
        internal INetworkService networkService;  //临时使用依赖注入的方法 
        internal FrameExecutor frameExecutor;
        
        //设置一下tick时间 
        internal int tick;
        internal float tickTime;
        internal float curTickTime; 

        internal FrameBuffer frameBuffer;
        internal bool IsUpdate; 

        internal void ReceivedData(SCFrameData scFrameData)
        { 
            for (int i = 0 ; i < scFrameData.FrameDataList.Count; i++) {
                frameBuffer.AddConfirmedFrame(scFrameData.FrameDataList[i]);
            } 
        } 
        
        FrameData frameData = null;
        internal void FixedTick()
        {
            if (!IsUpdate)
            {
                return;
            }
            curTickTime += Time.fixedDeltaTime;
            
            //进行输入采集 
            frameInputSample.InputSample(curTickTime,tickTime);  
            frameExecutor.Execute(frameData); 
            if (curTickTime >= tickTime)
            {
                frameData = frameBuffer.GetNextFrame(); 
                //打包发送
                CSFrameSample csFrameSample = frameInputSample.PackInput();
                csFrameSample.PlayerId = FrameContext.Context.CtrlRoleID;
                csFrameSample.RoomId = FrameContext.Context.SrvRoomID;
                csFrameSample.ClientCurFrame = frameBuffer.GetLastConfirmedFrameId();
                networkService.Send((int)MessageType.CscsframeSample,ProtoHelper.Serialize(csFrameSample)); 
                curTickTime -= tickTime; 
            }
        }

        //获取已经同步帧数
        internal int GetSyncFrame()
        {
            return frameBuffer.GetLastConfirmedFrameId();
        }

        internal void CloseFrameSync()
        {
            IsUpdate = false;
            frameInputSample.UnSubscribeEvent(); 
        } 
        
    }
}