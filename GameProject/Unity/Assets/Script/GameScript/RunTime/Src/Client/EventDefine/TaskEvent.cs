namespace MyGame
{
    public partial struct TaskEvent
    {
        public static readonly long TaskChange = "TaskEvent_Change".StringToHash(); 
        public static readonly long TaskSetCurProgress = "TaskEvent_SetCurProgress".StringToHash();
        public static readonly long TaskAddProgress = "TaskEvent_AddProgress".StringToHash();
        public static readonly long TaskAddNeeCheckProgress = "TaskEvent_AddNeeCheckProgress".StringToHash();
        public static readonly long TaskLunch = "TaskEvent_LunchProgress".StringToHash();
    }
}