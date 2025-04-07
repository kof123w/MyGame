using System;
using System.Collections.Generic; 

namespace MyGame
{
    public class GameTimerManager : Singleton<GameTimerManager>
    {
        private Dictionary<long,GameTimerAct> timerDict; 
        private List<long> needDelete = new();
        //计时器环境初始化
        public void Init()
        {
            timerDict = new Dictionary<long, GameTimerAct>();
        }  
        public void Tick()
        {
            float t = UnityEngine.Time.deltaTime;
            needDelete.Clear();
            foreach (KeyValuePair<long,GameTimerAct> actObjPair in timerDict) 
            {
                var actObj = actObjPair.Value;
                if (actObj.IsFrameTime)
                {
                    actObj.CurProgress += 1;
                }
                else
                {
                    actObj.CurProgress += t;
                }

                if (actObj.CurProgress >= actObj.TotalProgress)
                {
                    actObj.CurProgress = 0;
                    actObj.CurrentCnt += 1;
                    actObj.Action();
                    if (actObj.TotalCnt!=-1 && actObj.CurrentCnt >= actObj.TotalCnt)
                    {
                        needDelete.Add(actObjPair.Key);
                    }
                }
            }

            foreach (long deleteKey in needDelete)
            {
                if (timerDict.ContainsKey(deleteKey))
                {
                    var timerAct = timerDict[deleteKey]; 
                    timerDict.Remove(deleteKey);
                    Pool.Free(timerAct);
                }
            }
        }

        private void DelaySecondRun(string source,float second, Action action,int runCount = 1)
        {
            GameTimerAct gameTimerAct = Pool.Malloc<GameTimerAct>();
            gameTimerAct.Action = action;
            gameTimerAct.TotalProgress = second;
            gameTimerAct.TotalCnt = runCount;
            gameTimerAct.IsFrameTime = false;
            gameTimerAct.CurProgress = 0;
            if (!timerDict.TryAdd(source.StringToHash(), gameTimerAct))
            {
                DLogger.Error($"Create Timer Error,{source} key already exists!"); 
            }
        }

        private void DelayFrameRun(string source,float frame, Action action,int runCount = 1)
        {
            GameTimerAct gameTimerAct = Pool.Malloc<GameTimerAct>();
            gameTimerAct.Action = action;
            gameTimerAct.TotalProgress = frame;
            gameTimerAct.TotalCnt = runCount;
            gameTimerAct.IsFrameTime = false;
            gameTimerAct.CurProgress = 0;
            if (!timerDict.TryAdd(source.StringToHash(), gameTimerAct))
            {
                DLogger.Error($"Create Timer Error,{source} key already exists!"); 
            }
        }

        private void FreeTimer01(string source)
        {
            var deleteKey = source.StringToHash();
            if (timerDict.ContainsKey(deleteKey))
            {
                var timerAct = timerDict[deleteKey]; 
                timerDict.Remove(deleteKey);
                Pool.Free(timerAct);
            }
        }

        private bool TimerIsFree(string source)
        {
            return  !timerDict.ContainsKey(source.StringToHash());
        }

        public static void CreateLoopSecondTimer(string source,float second, Action action)
        {
            Instance.DelaySecondRun(source,second, action,-1);
        }

        public static void CreateSecondTimer(string source,float second, Action action, int runCount = 1)
        {
            Instance.DelaySecondRun(source,second, action,runCount);
        }
        
        public static void CreateLoopFrameTimer(string source,float frame, Action action)
        {
            Instance.DelayFrameRun(source,frame, action,  -1);
        }
        
        public static void CreateFrameTimer(string source,float second, Action action, int runCount=1)
        {
            Instance.DelayFrameRun(source,second, action,runCount);
        }

        public static void FreeTimer(string source)
        {
            Instance.FreeTimer01(source);
        }

        public static bool IsFreeTimer(string source)
        {
            return Instance.TimerIsFree(source);
        }  
    }
}