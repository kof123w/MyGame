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
       // receiver.Client.ReceiveTimeout = 5000; // 5秒
       // receiver.Client.SendTimeout = 3000; // 3秒
        //receiver.Ttl = (short)GameScope.DataCenter;
        Console.WriteLine($"UDP 服务器已启动，监听端口{port}");

        try
        {
            while (!cts.IsCancellationRequested)
            {
                try
                {
                    // 异步接收数据
                    var result = await receiver.ReceiveAsync().WithCancellation(cts.Token);
                    // 处理接收到的数据
                    ProcessReceivedData(result.Buffer, result.RemoteEndPoint);
                }
                catch (OperationCanceledException)
                {
                    // 正常关闭
                    break;
                }
                catch (SocketException ex) when (ex.SocketErrorCode == SocketError.ConnectionReset)
                {
                    // 客户端强制关闭
                    continue;
                }
                catch (ObjectDisposedException ex)
                {
                    // 明确被释放时需重建
                    receiver = new UdpClient(port);

                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
                catch (Exception ex)
                {
                    // 其他错误
                    Console.WriteLine($"错误: {ex.Message}");
                    // 根据严重程度决定是否继续
                    /*if (IsFatalError(ex.))
                        break;*/
                }

            }
        }
        finally
        {
            receiver.Close();
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

static class UdpServerExtra
{
    public static async Task<UdpReceiveResult> WithCancellation(this Task<UdpReceiveResult> task, CancellationToken cancellationToken)
    {
        var tcs = new TaskCompletionSource<bool>();
        using (cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs))
        {
            if (task != await Task.WhenAny(task, tcs.Task))
            {
                throw new OperationCanceledException(cancellationToken);
            }
        }
        return await task;
    }
}