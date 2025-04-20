using System;
using System.Buffers;
using System.Net.Sockets;
using System.Threading;
using Cysharp.Threading.Tasks;
using DebugTool;
using UnityEngine;

namespace MyGame
{
    public class AsyncTcpClient
    {
        private TcpClient tcpClient;
        private NetworkStream networkStream;
        //public event Action<Packet> OnMessageReceived;
        private CancellationTokenSource receiveCts;
        
        public bool IsConnected => tcpClient is { Connected: true };

        public async UniTask<bool> Connected(string serverIp, int serverPort)
        {
            try
            {
                tcpClient = new TcpClient();
                await tcpClient.ConnectAsync(serverIp, serverPort).AsUniTask().SuppressCancellationThrow();
                networkStream = tcpClient.GetStream();
                if (receiveCts?.IsCancellationRequested ?? false)
                {
                    receiveCts?.Cancel();
                    receiveCts?.Dispose();
                }
                receiveCts = new CancellationTokenSource();
                ReceivedData(receiveCts.Token).Forget();
                return true;
            }
            catch (Exception e)
            { 
                DLogger.Error(e.Message);
                return false;
            }
        }

        private async UniTask ReceivedData(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                // 1. 读取4字节长度前缀（使用ArrayPool）
                var prefixBuffer = ArrayPool<byte>.Shared.Rent(4);
                try
                {
                    await ReadExactAsync(networkStream, prefixBuffer, 4, token);
                    int bodyLength = BitConverter.ToInt32(prefixBuffer, 0);
            
                    // 安全校验
                    if (bodyLength <= 0 || bodyLength > 4906)
                    {
                        DLogger.Log($"非法消息长度: {bodyLength}");
                        Disconnect();
                        break;
                    }

                    // 2. 读取消息体（使用ArrayPool）
                    var bodyBuffer = ArrayPool<byte>.Shared.Rent(bodyLength);
                    try
                    {
                        await ReadExactAsync(networkStream, bodyBuffer, bodyLength, token); 
                        // 使用Span限定实际数据范围
                        var responseData = ProtoHelper.Deserialize<Packet>(bodyBuffer.AsSpan(0, bodyLength).ToArray());
                        NetManager.Instance.AddPacket(responseData);
                    }
                    finally
                    {
                        ArrayPool<byte>.Shared.Return(bodyBuffer);
                    }
                }
                catch (OperationCanceledException)
                {
                    // 正常取消
                    break;
                }
                catch (Exception ex)
                {
                    Debug.LogError($"接收数据异常: {ex}");
                    Disconnect();
                    break;
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(prefixBuffer);
                }
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
        
        public void Send(Packet packet)
        {
            var data = ProtoHelper.Serialize(packet);
            // 加上长度前缀（4字节，表示 data 长度）
            var lengthPrefix = BitConverter.GetBytes(data.Length);
            var finalData = new byte[4 + data.Length];
            Buffer.BlockCopy(lengthPrefix, 0, finalData, 0, 4);
            Buffer.BlockCopy(data, 0, finalData, 4, data.Length);

            networkStream.Write(finalData, 0, finalData.Length); 
        }
    }
}