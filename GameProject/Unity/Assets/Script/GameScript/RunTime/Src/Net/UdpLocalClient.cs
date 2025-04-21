using System;
using System.Net.Sockets;
using System.Threading;
using Cysharp.Threading.Tasks;
using DebugTool;

namespace MyGame
{
    public class UdpLocalClient : INetworkService
    {
        private UdpClient client;
        private readonly string serverIp;
        private readonly int serverPort;
        public event Action<Packet> OnReceived;
        private CancellationTokenSource udpReceiveToken;
        public UdpLocalClient(string serverIp, int serverPort)
        {
            this.serverIp = serverIp;
            this.serverPort = serverPort;
            client = new UdpClient(); 
            client.Connect(serverIp, serverPort);
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
            while (!token.IsCancellationRequested)
            {
                var result = await client.ReceiveAsync().ConfigureAwait(false);
                Packet packet = ProtoHelper.Deserialize<Packet>(result.Buffer);  
                NetManager.Instance.AddPacket(packet);
                OnReceived?.Invoke(packet);
            }
        }

        public void Send(byte[] data)
        {
            try
            {
                client.Send(data, data.Length);
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
            OnReceived = null;
            client?.Close();
        }
    }
}