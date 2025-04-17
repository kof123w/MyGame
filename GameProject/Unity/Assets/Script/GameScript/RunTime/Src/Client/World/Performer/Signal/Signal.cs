using EventHash;

namespace MyGame
{
    //信号命令
    public struct SignalEvent
    {
        public static long SignalControl_MoveSignal = "SignalControl_MoveSignal".StringToHash();  //移动信号
        public static long SignalControl_CameraMoveSignal = "SignalControl_CameraMoveSignal".StringToHash(); //摄像机移动信号
    }
    
    public enum SignalEnum
    {
        MoveSignal,
        CameraMoveSignal,
    }
}