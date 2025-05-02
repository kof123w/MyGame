using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DebugTool;
using MyGame;

namespace Config
{
    class ResDictionary<T1,T2> where T2 : new ()
    {
        private Dictionary<T1, T2> dict = new Dictionary<T1, T2>();
        private CacheObject<T2> cacheObject = null;
        private bool initialized = false;
        public List<T2> GetCacheList
        {
            get { return cacheObject.CacheList; }
        }
 
        public ResDictionary(CacheObject<T2> cacheObject)
        {
            this.cacheObject = cacheObject;
        }

        public async void Init(Func<T2,T1> func)
        {
            if (func == null)
            {
                DLogger.Error($"init config key failed.calcKey can`t null !");
                return;
            }

            if (cacheObject != null)
            {
                await UniTask.WaitUntil(() => cacheObject.IsCompleteLoad);
                for (int i = 0; i < cacheObject.CacheList.Count; i++)
                {
                    T2 t2 = cacheObject.CacheList[i];
                    T1 t1 = func(t2);
                    AddVal(t1, t2);
                }
                initialized = true;
            }
        }

        private void AddVal(T1 key,T2 val)
        {
            bool res = dict.TryAdd(key, val);
            DLogger.Log($"add key: {key} val: {val.ToString()}");
            if (!res)
            {
                DLogger.Error($"add config key failed.key {key} !");
            }
        }

        public async UniTask<T2> TryGetVal(T1 key)
        {
            await UniTask.WaitUntil(() => initialized);
            foreach (T2 t in cacheObject.CacheList)
            {
                DLogger.Log($"key: {key} val: {t.ToString()}");
            }

            foreach (var pair in dict)
            {
                DLogger.Log($"1111111111key: {pair.Key} val: {pair.Value.ToString()}");
            }

            if (dict.TryGetValue(key,out var t2))
            {
                return t2;
            }
            
            DLogger.Error($"try get config key failed.the key {key} !");
            return default(T2);
        }
    }
}
