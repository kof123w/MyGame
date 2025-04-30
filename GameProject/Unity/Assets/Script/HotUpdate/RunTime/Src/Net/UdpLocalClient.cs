using System;
using System.Buffers;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Cysharp.Threading.Tasks;
using DebugTool;
using UnityEngine;

namespace MyGame
{
    public class UdpLocalClient 
    {
        private UdpClient client;
        private readonly string serverIp;
        private readonly int serverPort;
        private IPEndPoint endPoint;
        private CancellationTokenSource udpReceiveToken;
        public UdpLocalClient(string serverIp, int serverPort)
        {
            this.serverIp = serverIp;
            this.serverPort = serverPort;
            client = new UdpClient();   
            //client.Connect(serverIp, serverPort);
            endPoint = new IPEndPoint(IPAddress.Parse(serverIp), serverPort); 
        } 
        
        public void StartReceive()
        {
            if (udpReceiveToken?.IsCancellationRequested == false)
            {
                udpReceiveToken?.Cancel();
                udpReceiveToken?.Dispose();
            } 
            udpReceiveToken = new CancellationTokenSource();
            UdpReceive(udpReceiveToken.Token).Forget();
        }

        private async UniTask UdpReceive(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    var result = await client.ReceiveAsync().ConfigureAwait(false);
                    //NetManager.Instance.AddPacket(result.Buffer); 
                    UDPNetManager.Instance.HandlerDispatch(result.Buffer);
                }
            }
            catch
            {
                // ignored
            }
        }

        public void Send(int messageType,byte[] data)
        {
            try
            { 
                var messageTypePrefix = BitConverter.GetBytes((int)messageType);
                var finalData = ArrayPool<byte>.Shared.Rent(4 + data.Length); 
                Buffer.BlockCopy(messageTypePrefix, 0, finalData, 0, 4);
                Buffer.BlockCopy(data, 0, finalData, 4, data.Length); 
                client.Send(finalData, 4 + data.Length,endPoint);
                ArrayPool<byte>.Shared.Return(finalData);
            }
            catch (Exception ex)
            {
                DLogger.Log(ex.Message);
            }
        } 

        public void Close()
        {
            if (udpReceiveToken?.IsCancellationRequested == false)
            {
                udpReceiveToken?.Cancel();
                udpReceiveToken?.Dispose();
            }  
            client?.Close();
        }
    }
}