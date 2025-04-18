using EventHash;

namespace MyGame
{
    public partial struct UIEvent
    {
        public static long UIEventOpenTitleUI = "UIEvent_OpenTitleUI".StringToHash(); 
        public static long UIEventCloseTitleUI = "UIEvent_CloseTitleUI".StringToHash(); 
    }
}