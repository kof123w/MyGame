using System;
using MyGame;

namespace GameTimer
{
    public class GameTimerAct : IMemoryPool
    {
        public Action Action { get; internal set; }

        public float CurProgress { get; internal set; }

        public float TotalProgress { get; internal set; }
        
        public int TotalCnt { get; internal set; }  //执行次数
        public int CurrentCnt { get; internal set; } 

        public bool IsFrameTime { get; internal set; }
        
        public GameTimerAct(float totalProgress,Action action)
        {
            Action = action;
            TotalProgress = totalProgress;
        }

        public GameTimerAct()
        {
            
        }

        public void OnDestroy()
        {
             
        }
    }
}

