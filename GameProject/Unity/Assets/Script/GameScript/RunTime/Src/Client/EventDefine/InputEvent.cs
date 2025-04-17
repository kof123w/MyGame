using EventHash;

namespace MyGame
{
    public struct InputEvent
    {
        //键盘输入
        public static readonly long KeyboardHold = "InputEvent_KeyboardHold".StringToHash();
        
        //鼠标滑动接听
        public static readonly long MoveMouse = "InputEvent_MoveMouse".StringToHash();
    }
}