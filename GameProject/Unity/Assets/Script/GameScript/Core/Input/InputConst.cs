namespace MyGame
{
    public enum InputCmd
    {
        None,  
        MoveForward,//前
        BackForward, //后
        Left,    //左
        Right,   //右
        Move,    //遥杆
        Run,     //奔跑
        BackQuote,  //'`'按键
    }

    public class InputEvent
    {
        public static long KeyHold = "InputMgr_KeyHold".StringToHash();
        public static long KeyDown = "InputMgr_KeyDown".StringToHash();
        public static long KeyUp = "InputMgr_KeyUp".StringToHash();
    }
}