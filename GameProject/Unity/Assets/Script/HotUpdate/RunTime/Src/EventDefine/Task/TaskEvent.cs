using EventHash;

namespace MyGame
{
    public struct TaskEvent
    {
        public static readonly long TaskChange = "TaskEvent_Change".StringToHash(); 
        public static readonly long TaskSetCurTask = "TaskEvent_SetCurTask".StringToHash();
        public static readonly long TaskAddTask = "TaskEvent_AddTask".StringToHash();
        public static readonly long TaskLunch = "TaskEvent_LunchTask".StringToHash();
    }
}