namespace MyGame
{
    public struct UIEvent
    {
        public static readonly long UIManagerEvent_LoadFinishWindow = "UIManagerEvent_LoadFinishWindow".StringToHash();
        public static readonly long UIManagerEvent_AddUIController = "UIManagerEvent_AddUIController".GetHashCode();
    }
}