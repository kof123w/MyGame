namespace MyGame
{
    public partial struct ProgressEvent
    {
        public static readonly long ProgressChange = "ProgressEvent_Change".StringToHash();
        public static readonly long ProgressMakeDirty = "ProgressEvent_MakeDirty".StringToHash();
        public static readonly long ProgressFinishDirt = "ProgressEvent_Finish".StringToHash();
        public static readonly long ProgressSetCurProgress = "ProgressEvent_SetCurProgress".StringToHash();
        public static readonly long ProgressAddProgress = "ProgressEvent_AddProgress".StringToHash();
        public static readonly long ProgressAddNeeCheckProgress = "ProgressEvent_AddNeeCheckProgress".StringToHash();
        public static readonly long ProgressLunch = "ProgressEvent_LunchProgress".StringToHash();
    }
}