using EventHash;

namespace MyGame
{
    public struct DataEvent
    {
        public static readonly long LoginEvent = "DataEvent_LoginEvent".StringToHash();
    }
}