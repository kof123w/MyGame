using System.Net;
using System.Net.Sockets;
using ConsoleApp1.TCPServer.Src.ServerParam;
using MyServer;

namespace MyGame;

[UDPUseTag(true)]
public class UDPHandler : INetHandler
{
    public void RegNet()
    { 
         HandlerDispatch.Instance.RegisterUdpHandler(MessageType.CscsframeSample,FrameSampleHandle);
         HandlerDispatch.Instance.RegisterUdpHandler(MessageType.CspostClientUdpAddress,PostPlayerAddress); 
    } 

    private void PostPlayerAddress(IPEndPoint clientAddress, byte[] packet)
    {
        var postClientUdpAddress = ProtoHelper.Deserialize<CSPostClientUdpAddress>(packet); 
        RoomLogic.Instance.SetRoomPlayerState(postClientUdpAddress.RoomId,postClientUdpAddress.PlayerId,clientAddress);
    }

    private void FrameSampleHandle(IPEndPoint clientAddress, byte[] packet)
    {
        var frameSample = ProtoHelper.Deserialize<CSFrameSample>(packet);
        RoomLogic.Instance.SamplePlayerHandler(frameSample);
    }
}