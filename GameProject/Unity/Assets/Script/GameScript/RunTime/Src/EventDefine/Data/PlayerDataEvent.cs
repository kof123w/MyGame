using EventHash;

namespace MyGame
{
    public partial struct DataEvent
    {
        public static readonly long PlayerDataEventSetData = "DataEven_SetPlayerData".StringToHash(); 
    }
}