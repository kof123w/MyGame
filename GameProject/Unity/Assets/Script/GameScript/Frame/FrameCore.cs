using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using BEPUphysics;
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
        
        //设置一下tick时间 
        internal int tick;
        internal Fix64 tickTime;
        internal Fix64 curTickTime; 

        internal FrameBuffer frameBuffer;
        internal bool IsUpdate; 

        internal void ReceivedData(SCFrameData scFrameData)
        { 
            for (int i = 0 ; i < scFrameData.FrameDataList.Count; i++) {
                frameBuffer.AddConfirmedFrame(scFrameData.FrameDataList[i]);
            } 
        } 

        internal void FixedTick()
        {
            if (!IsUpdate)
            {
                return;
            }
            curTickTime += Time.fixedDeltaTime;
            
            //进行输入采集 
            frameInputSample.InputSample(curTickTime,tickTime);  

            if (curTickTime >= tickTime)
            {
                frameBuffer.GetNextFrame(); 
                
                //打包发送
                CSFrameSample csFrameSample = frameInputSample.PackInput();
                csFrameSample.PlayerId = FrameContext.Context.CtrlRoleID;
                csFrameSample.RoomId = FrameContext.Context.SrvRoomID;
                csFrameSample.ClientCurFrame = frameBuffer.GetLastConfirmedFrameId();
                networkService.Send((int)MessageType.CscsframeSample,ProtoHelper.Serialize(csFrameSample));
                curTickTime = 0.0f; 
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