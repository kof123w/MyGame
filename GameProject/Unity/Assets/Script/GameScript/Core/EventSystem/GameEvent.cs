using System;

namespace MyGame
{
    public static class GameEvent
    { 
        public static void Subscribe(this Object obj,long id,Action action)
        {
           EventListenManager.Instance.AddListener(obj,id,action);
        }
        
        public static void Subscribe<T>(this Object obj,long id,Action<T> action)
        {
           EventListenManager.Instance.AddListener<T>(obj,id,action);
        }
        
        public static void Subscribe<T1,T2>(this Object obj,long id,Action<T1,T2> action)
        {
           EventListenManager.Instance.AddListener<T1,T2>(obj,id,action);
        }
        
        public static void Subscribe<T1,T2,T3>(this Object obj,long id,Action<T1,T2,T3> action)
        {
           EventListenManager.Instance.AddListener<T1,T2,T3>(obj,id,action);
        }
        
        public static void Subscribe<T1,T2,T3,T4>(this Object obj,long id,Action<T1,T2,T3,T4> action)
        {
           EventListenManager.Instance.AddListener<T1,T2,T3,T4>(obj,id,action);
        }
    
        public static void UnSubscribe(this Object obj,long id)
        {
           EventListenManager.Instance.Unsubscribe(obj,id);
        }
    
        public static void Push(long id)
        {
           EventListenManager.Instance.Push(id);
        }
        
        public static void Push<T>(long id,T t)
        {
           EventListenManager.Instance.Push<T>(id,t);
        }
        
        public static void Push<T1,T2>(long id,T1 t1,T2 t2)
        {
           EventListenManager.Instance.Push<T1,T2>(id,t1,t2);
        }
        
        public static void Push<T1,T2,T3>(long id,T1 t1,T2 t2,T3 t3)
        {
           EventListenManager.Instance.Push<T1,T2,T3>(id,t1,t2,t3);
        }
        
        public static void Push<T1,T2,T3,T4>(long id,T1 t1,T2 t2,T3 t3,T4 t4)
        {
           EventListenManager.Instance.Push<T1,T2,T3,T4>(id,t1,t2,t3,t4);
        } 
    }
}

