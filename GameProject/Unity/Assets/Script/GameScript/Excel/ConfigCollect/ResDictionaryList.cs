using System;
using System.Collections.Generic;
using Config;
using UnityEngine;

namespace MyGame
{
    class ResDictionaryList<T1,T2>
    {
        private Dictionary<T1, List<T2>> m_dictList = new Dictionary<T1,List<T2>>();
        private CacheObject<T2> m_cacheObject = null;
        public ResDictionaryList()
        {
            m_cacheObject = CacheObject<T2>.Instance;
        }

        public List<T2> GetCacheList
        {
            get { return m_cacheObject.CacheList; }
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
                    List<T2> t2List = TryGetVal(t1);
                    if (t2List == null)
                    {
                        t2List = new List<T2>();
                        m_dictList.Add(t1,t2List);
                    }
                    
                    t2List.Add(t2);
                }
            }
        } 

        public List<T2> TryGetVal(T1 key)
        {
            if (m_dictList.TryGetValue(key,out var t2List))
            {
                return t2List;
            }
            
            DLogger.Error($"add config key failed.key {key} !");
            return default(List<T2>);
        }
    }
}
