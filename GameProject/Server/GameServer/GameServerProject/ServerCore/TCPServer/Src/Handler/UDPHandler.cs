using System.Net;
using MyServer;

namespace MyGame;

[UDPUseTag(true)]
public class UDPHandler : INetHandler
{
    public void RegNet()
    {
         HandlerDispatch.Instance.RegisterUdpHandler(MessageType.CsjoinRoom,JoinRoomHandle);
    }

    private void JoinRoomHandle(IPEndPoint clientAddress,byte[] packet)
    {
        CSJoinRoom joinRoom = ProtoHelper.Deserialize<CSJoinRoom>(packet);
        int index = RoomLogic.Instance.JoinRoom(joinRoom.RoomId, clientAddress);
        int randomSeed = RoomLogic.Instance.GetRoomRandomSeed(joinRoom.RoomId);
        SCJointRoom scJointRoom = new SCJointRoom();
        scJointRoom.ResuleCode = index > 0 ? ResuleCode.Finished : ResuleCode.Failed;
        scJointRoom.PlayerIndex = index;
        scJointRoom.RandomSeed = randomSeed;
        _ = UDPServer.Instance.SendAsync( MessageType.ScjoinRoom,scJointRoom, clientAddress);
    }
}