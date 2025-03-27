using System;
using System.Collections.Generic;

namespace MyGame
{
    public class TimerManger : UnitySingleton<TimerManger>
    {
        private Dictionary<long,TimerAct> m_timerDict; 

        //计时器环境初始化
        public void Init()
        {
            m_timerDict = new Dictionary<long, TimerAct>();
        } 
        
        private List<long> m_needDelete = new();
        private void Update()
        {
            float t = UnityEngine.Time.deltaTime;
            m_needDelete.Clear();
            foreach (KeyValuePair<long,TimerAct> actObjPair in m_timerDict) 
            {
                var actObj = actObjPair.Value;
                actObj.CurProgress += t;
                if (actObj.CurProgress >= actObj.TotalProgress)
                {
                    actObj.CurrentCnt += 1;
                    actObj.Action();
                    if (actObj.TotalCnt!=-1 && actObj.CurrentCnt >= actObj.TotalCnt)
                    {
                        m_needDelete.Add(actObjPair.Key);
                    }
                }
            }

            foreach (long deleteKey in m_needDelete)
            {
                if (m_timerDict.ContainsKey(deleteKey))
                {
                    var timerAct = m_timerDict[deleteKey]; 
                    m_timerDict.Remove(deleteKey);
                    Pool.Free(timerAct);
                }
            }
        }

        private void DelaySecondRun(string source,float second, Action action,int runCount = 1)
        {
            TimerAct timerAct = Pool.Malloc<TimerAct>();
            timerAct.Action = action;
            timerAct.TotalProgress = second;
            timerAct.TotalCnt = runCount;
            timerAct.IsFrameTime = false;
            if (!m_timerDict.TryAdd(source.StringToHash(), timerAct))
            {
                DLogger.Error($"Create Timer Error,{source} key already exists!"); 
            }
        }

        private void DelayFrameRun(string source,float frame, Action action,int runCount = 1)
        {
            TimerAct timerAct = Pool.Malloc<TimerAct>();
            timerAct.Action = action;
            timerAct.TotalProgress = frame;
            timerAct.TotalCnt = runCount;
            timerAct.IsFrameTime = false;
            if (!m_timerDict.TryAdd(source.StringToHash(), timerAct))
            {
                DLogger.Error($"Create Timer Error,{source} key already exists!"); 
            }
        }

        private void FreeTimer(string source)
        {
            var deleteKey = source.StringToHash();
            if (m_timerDict.ContainsKey(deleteKey))
            {
                var timerAct = m_timerDict[deleteKey]; 
                m_timerDict.Remove(deleteKey);
                Pool.Free(timerAct);
            }
        }

        private bool TimerIsFree(string source)
        {
            return m_timerDict.ContainsKey(source.StringToHash());
        }

        public static void CreateLoopSecondTimer(string source,float second, Action action)
        {
            if (Instance != null)
            {
                Instance.DelaySecondRun(source,second, action,-1);
            }
        }
        
        public static void CreateSecondTimer(string source,float second, Action action, int runCount = 1)
        {
            if (Instance != null)
            {
                Instance.DelaySecondRun(source,second, action,runCount);
            }
        }
        
        public static void CreateLoopFrameTimer(string source,float frame, Action action)
        {
            if (Instance != null)
            {
                Instance.DelayFrameRun(source,frame, action,  -1);
            }
        }
        
        public static void CreateFrameTimer(string source,float second, Action action, int runCount=1)
        {
            if (Instance != null)
            {
                Instance.DelayFrameRun(source,second, action,runCount);
            }
        }

        public static void ReleaseTimer(string source)
        {
            if (Instance != null)
            {
                Instance.FreeTimer(source);
            }
        }

        public static void IsFreeTimer(string source)
        {
            if (Instance != null)
            {
                Instance.TimerIsFree(source);
            }
        }
    }
}