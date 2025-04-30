using System; 
using System.Collections.Generic;
using DebugTool; 
using SingleTool;

namespace EventSystem
{
    public class EventListenManager : Singleton<EventListenManager>,IEventManger
    { 
        public Dictionary<long,Dictionary<object,Delegate>> eventRegisterDict;  
        
        public void Init()
        {
            DLogger.Log("==============>Start to init EventSysManager!");
            eventRegisterDict = new Dictionary<long,Dictionary<object,Delegate>>();
            eventRegisterDict.Clear();
        }
        

        #region AddListener for wrap call back

        public void AddListener(Object obj,long eventId, Action act)
        {
            Dictionary<Object,Delegate> tmpDel;
            if (eventRegisterDict.TryGetValue(eventId, out tmpDel))
            {
                tmpDel.Add(obj,act);
            }
            else
            {
                tmpDel = new Dictionary<Object,Delegate>();
                tmpDel.Add(obj,act);
                eventRegisterDict.Add(eventId,tmpDel);
            }
        }

        public void AddListener<T>(Object obj,long eventId, Action<T> act)
        {
            Dictionary<Object,Delegate> tmpDel;
            if (eventRegisterDict.TryGetValue(eventId, out tmpDel))
            {
                tmpDel.Add(obj,act);
            }
            else
            {
                tmpDel = new Dictionary<Object,Delegate>();
                tmpDel.Add(obj,act);
                eventRegisterDict.Add(eventId,tmpDel);
            }
        }

        public void AddListener<T1,T2>(Object obj,long eventId, Action<T1,T2> act)
        {
            Dictionary<Object,Delegate> tmpDel;
            if (eventRegisterDict.TryGetValue(eventId, out tmpDel))
            {
                tmpDel.Add(obj,act);
            }
            else
            {
                tmpDel = new Dictionary<Object,Delegate>();
                tmpDel.Add(obj,act);
                eventRegisterDict.Add(eventId,tmpDel);
            }
        }

        public void AddListener<T1, T2, T3>(Object obj,long eventId, Action<T1, T2, T3> act)
        {
            Dictionary<Object,Delegate> tmpDel;
            if (eventRegisterDict.TryGetValue(eventId, out tmpDel))
            {
                tmpDel.Add(obj,act);
            }
            else
            {
                tmpDel = new Dictionary<Object,Delegate>();
                tmpDel.Add(obj,act);
                eventRegisterDict.Add(eventId,tmpDel);
            }
        }

        public void AddListener<T1, T2, T3, T4>(Object obj,long eventId, Action<T1, T2, T3, T4> act)
        {
            Dictionary<Object,Delegate> tmpDel;
            if (eventRegisterDict.TryGetValue(eventId, out tmpDel))
            {
                tmpDel.Add(obj,act);
            }
            else
            {
                tmpDel = new Dictionary<Object,Delegate>();
                tmpDel.Add(obj,act);
                eventRegisterDict.Add(eventId,tmpDel);
            }
        }

        #endregion

        #region clear event

        public void Unsubscribe(Object obj,long eventId)
        {
            Dictionary<Object,Delegate> tmpDel;
            if (eventRegisterDict.TryGetValue(eventId, out tmpDel))
            {
                if (tmpDel.ContainsKey(obj))
                {
                    tmpDel.Remove(obj);
                }
            }
        }

        public void ClearAllEventRegister()
        {
            eventRegisterDict.Clear();
        }

        #endregion
        
        #region observation action

        public void Push(long eventId)
        {
            Dictionary<object,Delegate> tmpDel;
            if (eventRegisterDict.TryGetValue(eventId, out tmpDel))
            {
                var itor = tmpDel.GetEnumerator();
                while (itor.MoveNext())
                {
                    var val = itor.Current.Value;
                    Action action = val as Action;
                    if (action != null)
                    {
                        action();
                    }
                }
            } 
        }
        
        public void Push<T>(long eventId,T t)
        {
            Dictionary<object,Delegate> tmpDel;
            if (eventRegisterDict.TryGetValue(eventId, out tmpDel))
            {
                var itor = tmpDel.GetEnumerator();
                while (itor.MoveNext())
                {
                    var val = itor.Current.Value;
                    Action<T> action = val as Action<T>;
                    if (action != null)
                    {
                        action(t);
                    }
                }
            } 
        }
        
        public void Push<T1,T2>(long eventId,T1 t1,T2 t2)
        {
            Dictionary<object,Delegate> tmpDel;
            if (eventRegisterDict.TryGetValue(eventId, out tmpDel))
            {
                var itor = tmpDel.GetEnumerator();
                while (itor.MoveNext())
                {
                    var val = itor.Current.Value;
                    Action<T1,T2> action = val as Action<T1,T2>;
                    if (action != null)
                    {
                        action(t1,t2);
                    }
                }
            } 
        }
        
        public void Push<T1,T2,T3>(long eventId,T1 t1,T2 t2,T3 t3)
        {
            Dictionary<object,Delegate> tmpDel;
            if (eventRegisterDict.TryGetValue(eventId, out tmpDel))
            {
                var itor = tmpDel.GetEnumerator();
                while (itor.MoveNext())
                {
                    var val = itor.Current.Value;
                    Action<T1,T2,T3> action = val as Action<T1,T2,T3>;
                    if (action != null)
                    {
                        action(t1,t2,t3);
                    }
                }
            } 
        }
        
        public void Push<T1,T2,T3,T4>(long eventId,T1 t1,T2 t2,T3 t3,T4 t4)
        {
            Dictionary<object,Delegate> tmpDel;
            if (eventRegisterDict.TryGetValue(eventId, out tmpDel))
            {
                var itor = tmpDel.GetEnumerator();
                while (itor.MoveNext())
                {
                    var val = itor.Current.Value;
                    Action<T1,T2,T3,T4> action = val as Action<T1,T2,T3,T4>;
                    if (action != null)
                    {
                        action(t1,t2,t3,t4);
                    }
                }
            } 
        }

        #endregion
       
    } 
}