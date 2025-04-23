using System;
using System.Collections.Generic;
using DebugTool;
using MyGame;

namespace Config
{
    class ResDictionary<T1,T2> where T2 : new ()
    {
        private Dictionary<T1, T2> dict = new Dictionary<T1, T2>();
        private CacheObject<T2> cacheObject = null;
        
        public List<T2> GetCacheList
        {
            get { return cacheObject.CacheList; }
        }
 
        public ResDictionary(CacheObject<T2> cacheObject)
        {
            this.cacheObject = cacheObject;
        }

        public void Init(Func<T2,T1> func)
        {
            if (func == null)
            {
                DLogger.Error($"init config key failed.calcKey can`t null !");
                return;
            }

            if (cacheObject != null)
            {
                for (int i = 0; i < cacheObject.CacheList.Count; i++)
                {
                    T2 t2 = cacheObject.CacheList[i];
                    T1 t1 = func(t2);
                    AddVal(t1, t2);
                }
            }
        }

        private void AddVal(T1 key,T2 val)
        {
            bool res = dict.TryAdd(key, val);
            if (!res)
            {
                DLogger.Error($"add config key failed.key {key} !");
            }
        }

        public T2 TryGetVal(T1 key)
        {
            if (dict.TryGetValue(key,out var t2))
            {
                return t2;
            }
            
            DLogger.Error($"add config key failed.key {key} !");
            return default(T2);
        }
    }
}
