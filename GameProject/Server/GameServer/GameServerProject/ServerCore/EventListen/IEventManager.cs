namespace EventSystem
{
    //事件的观察者
    public interface IEventManger
    {
        public void AddListener(Object obj, long eventId, Action act);

        public void AddListener<T>(Object obj, long eventId, Action<T> act);

        public void AddListener<T1, T2>(Object obj, long eventId, Action<T1, T2> act);

        public void AddListener<T1, T2, T3>(Object obj, long eventId, Action<T1, T2, T3> act);

        public void AddListener<T1, T2, T3, T4>(Object obj, long eventId, Action<T1, T2, T3, T4> act);

        public void Unsubscribe(Object obj, long eventId);

        public void ClearAllEventRegister();

        public void Push(long eventId);
        public void Push<T>(long eventId, T t);

        public void Push<T1, T2>(long eventId, T1 t1, T2 t2);

        public void Push<T1, T2, T3>(long eventId, T1 t1, T2 t2, T3 t3);

        public void Push<T1, T2, T3, T4>(long eventId, T1 t1, T2 t2, T3 t3, T4 t4);
    }
}