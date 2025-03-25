using System;
using System.Collections.Generic;

namespace MyGame
{
    public class TimerManger : UnitySingleton<TimerManger>
{
    private List<TimerAct> m_onceSecondList = new ();
    private List<TimerAct> m_onceFrameList = new();
    private Dictionary<long,TimerAct> m_frameList = new();
    private Dictionary<long,TimerAct> m_secondList = new();

    //计时器环境初始化
    public void Init()
    {
    }

    private void Update()
    {
        float t = UnityEngine.Time.deltaTime;
        for (int i = m_onceSecondList.Count-1;i >= 0;i--)
        {
            var actObj = m_onceSecondList[i];
            actObj.CurProgress += t;
            if (actObj.CurProgress >= actObj.TotalProgress)
            {
                actObj.Action();
                m_onceSecondList.RemoveAt(i);
            }
        }
         
        for (int i = m_onceFrameList.Count-1;i >= 0;i--)
        {
            var actObj = m_onceFrameList[i];
            actObj.CurProgress += 1;
            if (actObj.CurProgress >= actObj.TotalProgress)
            {
                actObj.Action();
                m_onceFrameList.RemoveAt(i);
            }
        }
        
        for (int i = m_secondList.Count-1;i >= 0;i--)
        {
            var actObj = m_secondList[i];
            actObj.CurProgress += t;
            if (actObj.CurProgress >= actObj.TotalProgress)
            {
                actObj.Action();
                actObj.CurProgress = 0;
            }
            
        }
        
        for (int i = m_frameList.Count-1;i >= 0;i--)
        {
            var actObj = m_frameList[i];
            actObj.CurProgress += 1;
            if (actObj.CurProgress >= actObj.TotalProgress)
            {
                actObj.Action();
                actObj.CurProgress = 0;
            }
        }
    }

    public void DelaySecondExec(float second,Action action)
    {
        TimerAct timerAct = new TimerAct(second,action);
        m_onceSecondList.Add(timerAct);
    }

    public void DelayFrameExec(float frame,Action action)
    {
        TimerAct timerAct = new TimerAct(frame,action);
        m_onceSecondList.Add(timerAct);
    }
}
}


