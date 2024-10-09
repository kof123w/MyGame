using System;

namespace MyGame;

public static class GameEvent
{
    public static void Subscribe(this object obj,long id,Action action)
    {
        EventListenMgr.Instance.AddListener(obj,id,action);
    }
    
    public static void Subscribe<T>(this object obj,long id,Action<T> action)
    {
        EventListenMgr.Instance.AddListener<T>(obj,id,action);
    }
    
    public static void Subscribe<T1,T2>(this object obj,long id,Action<T1,T2> action)
    {
        EventListenMgr.Instance.AddListener<T1,T2>(obj,id,action);
    }
    
    public static void Subscribe<T1,T2,T3>(this object obj,long id,Action<T1,T2,T3> action)
    {
        EventListenMgr.Instance.AddListener<T1,T2,T3>(obj,id,action);
    }
    
    public static void Subscribe<T1,T2,T3,T4>(this object obj,long id,Action<T1,T2,T3,T4> action)
    {
        EventListenMgr.Instance.AddListener<T1,T2,T3,T4>(obj,id,action);
    }

    public static void UnSubscribe(this object obj,long id)
    {
        EventListenMgr.Instance.Unsubscribe(obj,id);
    }

    public static void Push(this object obj,long id)
    {
        EventListenMgr.Instance.Push(id);
    }
    
    public static void Push<T>(this object obj,long id,T t)
    {
        EventListenMgr.Instance.Push<T>(id,t);
    }
    
    public static void Push<T1,T2>(this object obj,long id,T1 t1,T2 t2)
    {
        EventListenMgr.Instance.Push<T1,T2>(id,t1,t2);
    }
    
    public static void Push<T1,T2,T3>(this object obj,long id,T1 t1,T2 t2,T3 t3)
    {
        EventListenMgr.Instance.Push<T1,T2,T3>(id,t1,t2,t3);
    }
    
    public static void Push<T1,T2,T3,T4>(this object obj,long id,T1 t1,T2 t2,T3 t3,T4 t4)
    {
        EventListenMgr.Instance.Push<T1,T2,T3,T4>(id,t1,t2,t3,t4);
    }
}