using System;

namespace MyGame
{
    public interface INetworkService
    {
        public void Send(int type,byte[] data);
        public event Action<byte[]> OnReceived;
    }
}