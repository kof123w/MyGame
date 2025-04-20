using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MyServer;

public class UDPServer
{
    private UdpClient receiver;
    private UdpClient sender;
    private CancellationTokenSource cts;
    private readonly int port;
    private readonly string multicastIp = "239.192.10.1";

    public UDPServer(int port)
    {
        this.port = port;
        
    }

    public async Task StartAsync()
    { 
        cts = new CancellationTokenSource();
        receiver = new UdpClient(port);
        // 选择组织内部的多播地址（不会与公共协议冲突）
        IPAddress gameMulticastAddress = IPAddress.Parse(multicastIp); 
        receiver.JoinMulticastGroup(gameMulticastAddress);
        receiver.Client.ReceiveTimeout = 5000; // 5秒
        receiver.Client.SendTimeout = 3000; // 3秒
        receiver.Ttl = (short)GameScope.DataCenter;
        Console.WriteLine($"UDP 服务器已启动，监听端口{port}");
        try
        {
            // 异步接收数据
            var result = await receiver.ReceiveAsync().ConfigureAwait(false);
                
            // 处理接收到的数据
            ProcessReceivedData(result.Buffer, result.RemoteEndPoint);

        }
        catch (OperationCanceledException e)
        {
            Console.WriteLine(e);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            receiver.Close();
            Console.WriteLine("UDP 服务器已停止");
        }
    }
    
    //接收处理
    private void ProcessReceivedData(byte[] data, IPEndPoint remoteEP)
    {
        string message = Encoding.UTF8.GetString(data);
        Console.WriteLine($"收到来自 {remoteEP} 的消息: {message}");

        // 示例：发送回复
       // byte[] replyData = Encoding.UTF8.GetBytes($"已收到你的消息: {message}");
       // udpServer.Send(replyData, replyData.Length, remoteEP);
    }

    public async Task SendAsync(byte[] data)
    {
        sender = new UdpClient(); 
        await sender.SendAsync(data, data.Length, new IPEndPoint(IPAddress.Parse(multicastIp), port + 1));
        sender.Close(); 
    }

    public void Stop()
    {
        cts?.Cancel();
        sender?.Close();
        receiver?.Close();
    }
}