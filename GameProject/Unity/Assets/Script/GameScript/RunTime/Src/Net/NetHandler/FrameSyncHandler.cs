 

using System;

namespace MyGame
{
    public class FrameSyncHandler : INetHandler
    {
        public void RegNet()
        {
            NetManager.Instance.RegNetHandler(MessageType.ScjoinRoom,JoinRoomHandler);
        }

        private void JoinRoomHandler(Packet packet)
        {
            SCJointRoom scJointRoom = ProtoHelper.Deserialize<SCJointRoom>(packet);
            
        }
    }
}