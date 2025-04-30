using EventHash;

namespace MyGame
{
    //信号命令
    public struct InputSignal
    {
        public static long InputSignal_MoveSignal = "SignalControl_MoveSignal".StringToHash();  //移动信号
        public static long InputSignal_CameraMoveSignal = "SignalControl_CameraMoveSignal".StringToHash(); //摄像机移动信号
    }
    
    public enum SignalEnum
    {
        MoveSignal,
        CameraMoveSignal,
    }
}