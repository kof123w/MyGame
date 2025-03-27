namespace MyGame
{
    public partial struct ProgressEvent
    {
        public static readonly long ProgressChange = "ProgressEvent_Change".StringToHash();
        public static readonly long ProgressMakeDirty = "ProgressEvent_MakeDirty".StringToHash();
        public static readonly long ProgressFinishDirt = "ProgressEvent_Finish".StringToHash();
    }
}