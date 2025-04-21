using MyGame;
using SingleTool;
using UnityEngine;

public class UDPServer : Singleton<UDPServer>
{
    private UdpLocalClient udpLocalClient;
    private int curRoomId;
    
    public void Start(string relay)
    {
        string [] relayArr = relay.Split ('&');
        
        string ip = relayArr[0];
        int port = int.Parse(relayArr[1]);
        int roomId = int.Parse(relayArr[2]);
        curRoomId = roomId;
        udpLocalClient = new UdpLocalClient (ip, port);
        udpLocalClient.StartReceive();
        
        // todo .. 创建出来临时直接加入发加入房间
        CSJoinRoom csJoinRoom = new CSJoinRoom();
        csJoinRoom.RoomId = roomId;
        Packet packet = ProtoHelper.CreatePacket(MessageType.CsjoinRoom, csJoinRoom);
        udpLocalClient.Send(ProtoHelper.Serialize(packet));
    }
}
