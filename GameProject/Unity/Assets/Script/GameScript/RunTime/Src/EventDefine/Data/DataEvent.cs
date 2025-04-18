using EventHash;

namespace MyGame
{
    public partial struct DataEvent
    {
        public static readonly long LoginEvent = "DataEvent_LoginEvent".StringToHash();
    }
}