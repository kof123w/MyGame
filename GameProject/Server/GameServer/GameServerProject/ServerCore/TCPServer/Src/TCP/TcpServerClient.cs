using System.Buffers;
using System.Collections.Concurrent;
using System.Net.Sockets;
using Google.Protobuf;
using MyGame;

namespace MyServer;

public class TcpServerClient  
{
    private readonly TcpClient tcpClient;
    private PlayerServerData playerData;
    private readonly int id;
    private NetworkStream stream;
    private byte[] receiveBuffer;
    private const int BufferSize = 4096;
    private List<byte> receiveCache = new();

    public int Id => id;

    public TcpServerClient(int id, TcpClient tcpClient)
    {
        this.tcpClient = tcpClient;
        this.id = id;
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

            for (int i = 0; i < bytesRead; i++)
                receiveCache.Add(receiveBuffer[i]);

            // 粘包处理
            while (true)
            {
                if (receiveCache.Count < 4) break; // 不够读长度

                int msgLen = receiveCache.Count - 4;
                if (receiveCache.Count < 4 + msgLen) break; // 不够一条完整消息
                
                byte[] messageTypeData = receiveCache.Skip(4).Take(8).ToArray();
                MessageType msgType = (MessageType) BitConverter.ToInt32(messageTypeData, 0);
                byte[] fullMessage = receiveCache.Skip(8).Take(msgLen).ToArray();
                HandleData(msgType,fullMessage);
                receiveCache.RemoveRange(0, 4 + msgLen);
            }

            BeginReceive();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"客户端 {id} 处理数据错误: {ex.Message},{ex.StackTrace}");
            Disconnect();
        }
    }

    public void Send<T>(MessageType messageType, T t) where T : IMessage, new()
    {
        try
        {
            byte[] payload = ProtoHelper.Serialize(t);
            byte[] lengthPrefix = BitConverter.GetBytes(payload.Length + 4);
            byte[] messageTypePrefix = BitConverter.GetBytes((int)messageType);

            byte[] fullData = new byte[8 + payload.Length];
            Buffer.BlockCopy(lengthPrefix, 0, fullData, 0, 4);
            Buffer.BlockCopy(messageTypePrefix, 0, fullData, 4, 4);
            Buffer.BlockCopy(payload, 0, fullData, 8, payload.Length);

            stream.Write(fullData, 0, fullData.Length);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"向客户端 {id} 发送消息错误: {ex.Message}");
            Disconnect();
        }
    }

    private void HandleData(MessageType msgType,byte[] data)
    {  
        HandlerDispatch.Instance.Dispatch(this, data,msgType);
    }

    public void BindPlayer(PlayerServerData player)
    {
        playerData = player;
        playerData.SetTcpServerClient(this);
    }

    public PlayerServerData GetPlayer()
    {
        return playerData;
    }

    public void Disconnect()
    {
        try
        {
            Console.WriteLine($"客户端{id}IP{tcpClient.Client.RemoteEndPoint}断开连接");
            RoomLogic.Instance.ExitRoom(playerData.CurRoomID,playerData);
            
            stream?.Close();
            tcpClient?.Close();
            playerData.IsOnline = false;
            playerData = null;
            TCPServer.Instance.OnClientDisconnected(id);
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"客户端 {id} 断开连接时出错: {ex.Message} {ex.StackTrace}");
        }
    }
}