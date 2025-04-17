using System.Net.Sockets;

namespace MyServer;
public class Client
{
    private readonly int id;
    private readonly TcpClient tcpClient;
    private NetworkStream stream;
    private byte[] receiveBuffer;
    private const int BufferSize = 4096;

    public int Id => id;
    
    public Client(int id, TcpClient tcpClient)
    {
        this.id = id;
        this.tcpClient = tcpClient;
        stream = tcpClient.GetStream();
        receiveBuffer = new byte[BufferSize];
    }

    public void BeginReceive()
    {
        try
        {
            stream.BeginRead(receiveBuffer, 0, BufferSize, OnDataReceived, null);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"客户端 {id} 接收数据错误: {ex.Message}");
            Disconnect();
        }
    }

    private void OnDataReceived(IAsyncResult ar)
    {
        try
        {
            int bytesRead = stream.EndRead(ar);
            
            if (bytesRead <= 0)
            {
                Disconnect();
                return;
            }
            
            byte[] data = new byte[bytesRead];
            Array.Copy(receiveBuffer, data, bytesRead);
            
            // 处理接收到的数据
            HandleData(data);
            
            // 继续接收数据
            BeginReceive();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"客户端 {id} 处理数据错误: {ex.Message}");
            Disconnect();
        }
    }

    private void HandleData(byte[] data)
    {
        // 这里实现数据解析和处理逻辑
        // 示例：简单的字符串消息
        string message = System.Text.Encoding.UTF8.GetString(data);
        Console.WriteLine($"来自客户端 {id} 的消息: {message}");
        
        // 示例响应
        Send($"服务器已收到你的消息: {message}");
    }

    public void Send(string message)
    {
        try
        {
            byte[] data = System.Text.Encoding.UTF8.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"向客户端 {id} 发送消息错误: {ex.Message}");
            Disconnect();
        }
    }

    public void Disconnect()
    {
        try
        {
            stream?.Close();
            tcpClient?.Close();
            
            // 通知服务器此客户端已断开
            TCPServer.Instance.OnClientDisconnected(id);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"客户端 {id} 断开连接时出错: {ex.Message}");
        }
    }
}