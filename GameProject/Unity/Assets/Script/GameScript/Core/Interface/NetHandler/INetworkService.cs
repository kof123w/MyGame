using System;

namespace MyGame
{
    public interface INetworkService
    {
        public void Send(byte[] data);
        public event Action<byte[]> OnReceived;
    }
}