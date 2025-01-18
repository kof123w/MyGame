using System;

namespace MyGame
{
    public class TimerAct
    {
        public Action Action { get; private set; }

        public float CurProgress { get; set; }

        public float TotalProgress { get; private set; }

        public TimerAct(float totalProgress,Action action)
        {
            Action = action;
            TotalProgress = totalProgress;
        }
    }
}

