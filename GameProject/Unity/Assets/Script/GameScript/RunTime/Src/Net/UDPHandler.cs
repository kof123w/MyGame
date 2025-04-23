using System;

namespace MyGame
{
    [AttributeUsage(AttributeTargets.Class)]
    public class UDPHandler : Attribute
    {
        public bool UseUDP;

        public UDPHandler(bool useUDP)
        {
            UseUDP = useUDP;
        }
    }
}