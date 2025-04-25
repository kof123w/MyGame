 

using System;
using DebugTool;
using EventSystem;
using Google.Protobuf;
using UnityEngine;

namespace MyGame
{
    [UDPHandler(true)]
    public class FrameSyncHandler : INetHandler
    {
        public void RegNet()
        {  
            UDPNetManager.Instance.RegNetHandler(MessageType.ScframeData,SyncFrame);
        }

        private void SyncFrame(byte[] data)
        {
             SCFrameData scFrameData = ProtoHelper.Deserialize<SCFrameData>(data); 
             DLogger.Log($"收到帧数据: {scFrameData.FrameDataList[^1].Frame} 数据长度 {scFrameData.FrameDataList.Count} 客户端本地处理帧数:{FrameContext.Context.GetSyncFrame()}");
             GameEvent.Push(FrameSignal.Signal_FrameSync,scFrameData);
        }
    }
}