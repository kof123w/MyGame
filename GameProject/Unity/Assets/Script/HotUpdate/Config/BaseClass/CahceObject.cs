using System;
using System.Collections.Generic;

namespace Config
{
    class CacheObject<T>
    {
        public List<T> CacheList = new List<T>();
        public bool IsCompleteLoad = false;
        public void ReleaseCache()
        {
            CacheList.Clear();
        }
    }
}
