using EventHash;

namespace MyGame
{
    public struct TitleUIEvent
    {
        public static long UIEventOpenTitleUI = "UIEvent_OpenTitleUI".StringToHash(); 
        public static long UIEventCloseTitleUI = "UIEvent_CloseTitleUI".StringToHash(); 
        public static long UIEventTitleUILogin  = "UIEvent_TitleUILogin".StringToHash();  //进行登录
    }
}