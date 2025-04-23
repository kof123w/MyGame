using Google.Protobuf;
using MyServer;

namespace MyGame;

public class PlayerServerData
{
    public long RoleId { get; set; } 
    public string Account { get; set; }
    
    public bool IsOnline { get; set; }
    
    public int CurRoomID { get; set; }
    
    //临时这样放着了
    private TcpServerClient TcpServerClient { get; set; }

    public void SetTcpServerClient(TcpServerClient tcpServerClient)
    {
        TcpServerClient = tcpServerClient;
    }

    public void Send<T>(MessageType messageType, T t) where T : IMessage, new()
    {
        TcpServerClient.Send(messageType,t);
    }
}