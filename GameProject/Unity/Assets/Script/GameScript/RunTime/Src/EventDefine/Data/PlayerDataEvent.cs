using EventHash;

namespace MyGame
{
    public struct PlayerDataEvent
    {
        public static readonly long PlayerDataEventSetData = "DataEven_SetPlayerData".StringToHash(); 
    }
}