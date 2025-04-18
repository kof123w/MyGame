using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DebugTool;
using Google.Protobuf;

namespace MyGame
{
    public class TcpLocalClient
    {
        private TcpClient tcpClient;
        private NetworkStream networkStream;
        //public event Action<Packet> OnMessageReceived;

        public bool IsConnected => tcpClient is { Connected: true };

        public async UniTask<bool> Connected(string serverIp, int serverPort)
        {
            try
            {
                tcpClient = new TcpClient();
                await tcpClient.ConnectAsync(serverIp, serverPort);
                networkStream = tcpClient.GetStream();
                
                //启动接收线程
                //new Thread(ReceiveLoop).Start();
                await Task.Run(ReceiveLoop);
                return true;
            }
            catch (Exception e)
            { 
                DLogger.Error(e.Message);
                return false;
            }
        }
        
        public void Disconnect()
        {
            tcpClient?.Close();
        }
        
        private void ReceiveLoop()
        {
            try
            {
                while (tcpClient.Connected)
                {
                    // 读取消息长度前缀
                    var lengthBytes = new byte[4];
                    int read = networkStream.Read(lengthBytes, 0, 4);
                    if (read != 4) break;

                    int length = BitConverter.ToInt32(lengthBytes, 0);
                    if (length <= 0) continue;

                    // 读取消息内容
                    var buffer = new byte[length];
                    int totalRead = 0;
                    while (totalRead < length)
                    {
                        read = networkStream.Read(buffer, totalRead, length - totalRead);
                        if (read == 0) break;
                        totalRead += read;
                    }

                    if (totalRead < length) break;

                    // 解析消息
                    var packet = ProtoHelper.Deserialize<Packet>(buffer);
                    //OnMessageReceived?.Invoke(packet);
                    NetManager.Instance.AddPacket(packet);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Receive error: {ex.Message}");
            }
            finally
            {
                Disconnect();
            }
        }

        
        public void Send(MessageType messageType, IMessage message)
        {
            try
            {
                var packet = ProtoHelper.CreatePacket(messageType, message);
                var data = ProtoHelper.Serialize(packet);

                lock (networkStream)
                {
                    networkStream.Write(BitConverter.GetBytes(data.Length), 0, 4);
                    networkStream.Write(data, 0, data.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Send error: {ex.Message}");
            }
        }
    }
}