using System;
using System.Buffers;
using System.Net.Sockets;
using System.Threading;
using Cysharp.Threading.Tasks;
using DebugTool;
using Google.Protobuf;
using UnityEngine;

namespace MyGame
{
    public class TcpLocalClient
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
                bool isConnect = tcpClient.Connected;
                if (isConnect)
                {
                    networkStream = tcpClient.GetStream();
                    if (receiveCts?.IsCancellationRequested ?? false)
                    {
                        receiveCts?.Cancel();
                        receiveCts?.Dispose();
                    }
                    receiveCts = new CancellationTokenSource();
                    ReceivedData(receiveCts.Token).Forget();
                } 
                return isConnect;
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
                        NetManager.Instance.AddPacket(bodyBuffer.AsSpan(0, bodyLength).ToArray());
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
                    //Debug.LogError($"接收数据异常: {ex}");
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
            if (receiveCts?.IsCancellationRequested ?? false)
            {
                receiveCts?.Cancel();
                receiveCts?.Dispose();
            }
            tcpClient?.Close();
        }

        private byte[] responseBuffer = new byte[4096];
        
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
        
        public void Send(MessageType messageType,IMessage message)
        {
            var data = ProtoHelper.Serialize(message);
            // 加上长度前缀（4字节，表示 data 长度）
            var lengthPrefix = BitConverter.GetBytes(data.Length + 4);
            var messageTypePrefix = BitConverter.GetBytes((int)messageType);
            var finalData = new byte[8 + data.Length];
            Buffer.BlockCopy(lengthPrefix, 0, finalData, 0, 4);
            Buffer.BlockCopy(messageTypePrefix, 0, finalData, 4, 4);
            Buffer.BlockCopy(data, 0, finalData, 8, data.Length);
         
            networkStream.Write(finalData, 0, finalData.Length); 
        }
    }
}