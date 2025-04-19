using System.Net.Sockets;
using Google.Protobuf;
using MyGame;

namespace MyServer;
public class Client
    {
        protected readonly int id;
        protected NetworkStream stream;
        protected byte[] receiveBuffer;
        protected const int BufferSize = 4096;
        private List<byte> receiveCache = new();

        public int Id => id;

        public Client(int id, TcpClient tcpClient)
        {
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

                    int msgLen = BitConverter.ToInt32(receiveCache.ToArray(), 0);
                    if (receiveCache.Count < 4 + msgLen) break; // 不够一条完整消息

                    byte[] fullMessage = receiveCache.Skip(4).Take(msgLen).ToArray();
                    HandleData(fullMessage);
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

        public virtual void HandleData(byte[] data)
        {
            // todo ..
        }

        public void Send<T>(MessageType messageType, T t) where T : IMessage, new()
        {
            try
            {
                var packet = ProtoHelper.CreatePacket(messageType, t);
                byte[] payload = ProtoHelper.Serialize(packet);
                byte[] lengthPrefix = BitConverter.GetBytes(payload.Length);

                byte[] fullData = new byte[4 + payload.Length];
                Buffer.BlockCopy(lengthPrefix, 0, fullData, 0, 4);
                Buffer.BlockCopy(payload, 0, fullData, 4, payload.Length);

                stream.Write(fullData, 0, fullData.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"向客户端 {id} 发送消息错误: {ex.Message}");
                Disconnect();
            }
        }

        public virtual void Disconnect()
        {

        }
    }