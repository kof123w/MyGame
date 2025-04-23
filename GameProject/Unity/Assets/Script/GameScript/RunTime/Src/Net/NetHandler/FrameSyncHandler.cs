 

using System;
using DebugTool;
using Google.Protobuf;

namespace MyGame
{
    [UDPHandler(true)]
    public class FrameSyncHandler : INetHandler
    {
        public void RegNet()
        {
            //NetManager.Instance.RegNetHandler(MessageType.ScjoinRoom,JoinRoomHandler);
            UDPNetManager.Instance.RegNetHandler(MessageType.ScjoinRoom,JoinRoomHandler);
        }

        private void JoinRoomHandler(byte[] data)
        {
            SCJointRoom scJointRoom = ProtoHelper.Deserialize<SCJointRoom>(data);
            DLogger.Log($"JoinRoomHandler  scJointRoom.Tick:{scJointRoom.Tick} scJointRoom.playerIndex:{scJointRoom.PlayerIndex} scJointRoom.RandomSeed:{scJointRoom.RandomSeed}");
        }
    }
}