using EventHash;

namespace MyGame
{
    public partial struct NetEvent
    {
        public static readonly long LoginHandleEvent = "LoginHandler_Login".StringToHash();
        public static readonly long MatchHandleEvent = "MatchHandler_Login".StringToHash();
    }  
}