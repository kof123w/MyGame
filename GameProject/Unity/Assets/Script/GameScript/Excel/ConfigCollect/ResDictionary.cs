using System;
using System.Collections.Generic;
using MyGame;

namespace Config
{
    class ResDictionary<T1,T2> where T2 : new ()
    {
        private Dictionary<T1, T2> m_dict = new Dictionary<T1, T2>();
        private CacheObject<T2> m_cacheObject = null;
        
        public List<T2> GetCacheList
        {
            get { return m_cacheObject.CacheList; }
        }

        public ResDictionary()
        {
            m_cacheObject = CacheObject<T2>.Instance;
        }

        public void Init(Func<T2,T1> func)
        {
            if (func == null)
            {
                DLogger.Error($"init config key failed.calcKey can`t null !");
                return;
            }

            if (m_cacheObject != null)
            {
                for (int i = 0; i < m_cacheObject.CacheList.Count; i++)
                {
                    T2 t2 = m_cacheObject.CacheList[i];
                    T1 t1 = func(t2);
                    AddVal(t1, t2);
                }
            }
        }

        private void AddVal(T1 key,T2 val)
        {
            bool res = m_dict.TryAdd(key, val);
            if (!res)
            {
                DLogger.Error($"add config key failed.key {key} !");
            }
        }

        public T2 TryGetVal(T1 key)
        {
            if (m_dict.TryGetValue(key,out var t2))
            {
                return t2;
            }
            
            DLogger.Error($"add config key failed.key {key} !");
            return default(T2);
        }
    }
}
