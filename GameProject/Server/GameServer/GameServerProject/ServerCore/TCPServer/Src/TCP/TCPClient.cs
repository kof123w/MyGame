using System.Collections.Concurrent;
using System.Net.Sockets;
using MyGame;

namespace MyServer;

public class TCPClient : Client
{
    private readonly TcpClient tcpClient;

    public TCPClient(int id, TcpClient tcpClient) : base(id, tcpClient)
    {
        this.tcpClient = tcpClient;
    }

    public override void HandleData(byte[] data)
    {
        Packet packet = ProtoHelper.Deserialize<Packet>(data);
        HandlerDispatch.Instance.Dispatch(this, packet);
    }

    public override void Disconnect()
    {
        try
        {
            stream?.Close();
            tcpClient?.Close();
            TCPServer.Instance.OnClientDisconnected(id);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"客户端 {id} 断开连接时出错: {ex.Message},{ex.StackTrace}");
        }
    }
}