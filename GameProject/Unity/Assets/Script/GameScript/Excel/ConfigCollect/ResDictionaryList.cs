using System;
using System.Collections.Generic;
using Config;
using DebugTool;
using UnityEngine;

namespace MyGame
{
    class ResDictionaryList<T1,T2>
    {
        private Dictionary<T1, List<T2>> dictList = new Dictionary<T1,List<T2>>();
        private CacheObject<T2> cacheObject = null;
        public ResDictionaryList(CacheObject<T2> cache)
        {
            cacheObject = cache;
        }

        public List<T2> GetCacheList
        {
            get { return cacheObject.CacheList; }
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
                    List<T2> t2List = TryGetVal(t1);
                    if (t2List == null)
                    {
                        t2List = new List<T2>();
                        dictList.Add(t1,t2List);
                    }
                    
                    t2List.Add(t2);
                }
            }
        } 

        public List<T2> TryGetVal(T1 key)
        {
            if (dictList.TryGetValue(key,out var t2List))
            {
                return t2List;
            }
            
            DLogger.Error($"add config key failed.key {key} !");
            return default(List<T2>);
        }
    }
}
