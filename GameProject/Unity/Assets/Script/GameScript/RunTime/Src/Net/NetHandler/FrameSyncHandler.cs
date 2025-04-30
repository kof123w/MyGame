 

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
             GameEvent.Push(FrameSignal.Signal_FrameSync,scFrameData);
        }
    }
}