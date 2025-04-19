using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DebugTool;
using Google.Protobuf;

namespace MyGame
{
    public class AsyncTcpClient
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
                await tcpClient.ConnectAsync(serverIp, serverPort).AsUniTask().SuppressCancellationThrow();
                networkStream = tcpClient.GetStream();
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

        private byte[] responseBuffer = new byte[4096];
        public async UniTask<Packet> SendAsync(Packet packet,CancellationToken token)
        { 
            var data = ProtoHelper.Serialize(packet);
            // 加上长度前缀（4字节，表示 data 长度）
            var lengthPrefix = BitConverter.GetBytes(data.Length);
            var finalData = new byte[4 + data.Length];
            Buffer.BlockCopy(lengthPrefix, 0, finalData, 0, 4);
            Buffer.BlockCopy(data, 0, finalData, 4, data.Length);

            await networkStream.WriteAsync(finalData, 0, finalData.Length, CancellationTokenSource.CreateLinkedTokenSource(token).Token)
                .AsUniTask()
                .SuppressCancellationThrow();
            
            /*var (canceled,bytesRead) = 
                await networkStream.ReadAsync(responseBuffer, 0, responseBuffer.Length,CancellationTokenSource.CreateLinkedTokenSource(token).Token).
                AsUniTask().
                SuppressCancellationThrow();*/

            // 3. 读取 4 字节长度前缀
            var prefixBuffer = new byte[4];
            await ReadExactAsync(networkStream, prefixBuffer, 4, CancellationTokenSource.CreateLinkedTokenSource(token).Token);
            int bodyLength = BitConverter.ToInt32(prefixBuffer, 0);

            // 4. 读取 bodyLength 长度的数据
            var bodyBuffer = new byte[bodyLength];
            await ReadExactAsync(networkStream, bodyBuffer, bodyLength, CancellationTokenSource.CreateLinkedTokenSource(token).Token);
 
            // 5. 反序列化
            var responseData = ProtoHelper.Deserialize<Packet>(bodyBuffer);
            return responseData;
        }
        
        private static async UniTask ReadExactAsync(NetworkStream stream, byte[] buffer, int length, CancellationToken token)
        {
            int offset = 0;
            while (offset < length)
            {
                var (_,read) = await stream.ReadAsync(buffer, offset, length - offset, token).AsUniTask().SuppressCancellationThrow();
                if (read == 0)
                {
                    return;
                } 
                offset += read;
            }
        }
    }
}