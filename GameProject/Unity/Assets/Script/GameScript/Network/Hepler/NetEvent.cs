namespace MyGame
{
    public abstract partial class NetEvent
    {
        public static readonly long OpenWaitNetUI = "NetEvent_OpenWaitNetUI".GetHashCode();
        public static readonly long CloseWaitNetUI = "NetEvent_CloseWaitNetUI".GetHashCode();
    }
}