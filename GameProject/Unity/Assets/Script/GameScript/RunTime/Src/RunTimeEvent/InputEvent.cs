namespace MyGame
{
    public struct InputEvent
    {
        public static readonly long KeyHold = "InputEvent_KeyHold".StringToHash();
        public static readonly long KeyDown = "InputEvent_KeyDown".StringToHash();
        public static readonly long KeyUp = "InputEvent_KeyUp".StringToHash();
    }
}