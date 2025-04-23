using System.Buffers;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Google.Protobuf;
using MyGame;

namespace MyServer;

public class UDPServer : Singleton<UDPServer>
{
    private UdpClient receiver;
    private UdpClient sender;
    private CancellationTokenSource cts;
    private int port;
    private readonly string multicastIp = "239.192.10.1";

    public void SetPort(int port)
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
        
        while (true)
        {
            try
            { 
                // 异步接收数据
                var result = await receiver.ReceiveAsync().ConfigureAwait(false);
                
                // 处理接收到的数据
                ProcessReceivedData(result.Buffer, result.RemoteEndPoint);
            }
            catch (SocketException ex)
            {
                // 根据错误码判断是否需重建receiver
                if (IsFatalError(ex.SocketErrorCode))
                {
                    /*receiver.Dispose();
                    receiver = new UdpClient(port); // 或重新初始化Socket

                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);*/
                } 
            }
            catch (ObjectDisposedException)
            {
                // 明确被释放时需重建
                //  receiver = new UdpClient(port);
            }
            catch (Exception ex)
            {
                // 其他异常（如权限问题）可能需要终止或重建
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
           
        } 
        // ReSharper disable once FunctionNeverReturns
    }
    
    private bool IsFatalError(SocketError errorCode)
    {
        switch (errorCode)
        {
            // 需要关闭并重建receiver的致命错误
            case SocketError.AccessDenied:       // 权限丢失
            case SocketError.Shutdown:           // Socket已被关闭
            case SocketError.ConnectionReset:     // 远程强制关闭（某些UDP场景可能触发）
            case SocketError.NotSocket:           // 底层Socket已失效
            case SocketError.OperationAborted:   // 操作被取消（如Dispose后调用）
            case SocketError.SocketNotSupported: // 协议/地址族不支持
            case SocketError.ProtocolFamilyNotSupported:
            case SocketError.ProtocolNotSupported:
                return true;

            // 可恢复的临时错误（无需关闭）
            case SocketError.TimedOut:           // 超时
            case SocketError.NetworkDown:         // 网络不可用
            case SocketError.NetworkUnreachable:
            case SocketError.HostUnreachable:
            case SocketError.TryAgain:           // 临时资源不足
            case SocketError.MessageSize:         // 数据包过大（需调整缓冲区）
            default:
                return false;
        }
    }
     
    private void ProcessReceivedData(byte[] data, IPEndPoint remoteEP)
    { 
        //临时处理
        List<byte> bufferList = new List<byte>(); 
        bufferList.AddRange(data);

        var prefix = bufferList.Take(4).ToArray();
        var messageType = (MessageType)BitConverter.ToInt32(prefix, 0);
        
        Console.WriteLine($"收到来自 {remoteEP} 的消息: 协议号{messageType}");
        
       HandlerDispatch.Instance.Dispatch(remoteEP,bufferList.Skip(4).Take(bufferList.Count).ToArray(), messageType); 
    }

    public async Task MulticastSendAsync(byte[] data)
    {
        sender = new UdpClient(); 
        await sender.SendAsync(data, data.Length, new IPEndPoint(IPAddress.Parse(multicastIp), port + 1));
        sender.Close(); 
    }

    
    public async Task SendAsync(MessageType messageType,IMessage message, IPEndPoint remoteEp)
    {
        List<byte> bufferList = new List<byte>(); 
        var bytes = ProtoHelper.Serialize(message);
        bufferList.AddRange(BitConverter.GetBytes((int)messageType));
        bufferList.AddRange(bytes);
        await receiver.SendAsync(bufferList.ToArray(), bufferList.Count, remoteEp);
    }
    
    public void Send(MessageType messageType,IMessage message, IPEndPoint remoteEp)
    {
        List<byte> bufferList = new List<byte>(); 
        var bytes = ProtoHelper.Serialize(message);
        bufferList.AddRange(BitConverter.GetBytes((int)messageType));
        bufferList.AddRange(bytes);
        receiver.Send(bufferList.ToArray(), bufferList.Count, remoteEp);
    }

    public void Stop()
    {
        cts?.Cancel();
        sender?.Close();
        receiver?.Close();
    }
}