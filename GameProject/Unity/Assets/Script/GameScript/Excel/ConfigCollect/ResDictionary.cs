using System;
using System.Collections.Generic;
using Config;
using UnityEngine;

namespace MyGame
{
    class ResDictionary<T1,T2>
    {
        private Dictionary<T1, T2> m_dict = new Dictionary<T1, T2>();
        private CacheObject<T2> m_cacheObject = null;

        public ResDictionary(CacheObject<T2> cacheObject)
        {
            m_cacheObject = cacheObject;
        }

        public void Init(T1 param,Func<T1,T1> calcKey)
        {
            if (calcKey == null)
            {
                DLogger.Error($"init config key failed.calcKey can`t null !");
                return;
            }

            T1 key = calcKey(param);
        }

        private void AddVal(T1 key,T2 val)
        {
            bool res = m_dict.TryAdd(key, val);
            if (!res)
            {
                DLogger.Error($"add config key failed.key {key} !");
            }
        }
    }
}
