using System.Collections.Generic;

namespace Config
{
    class CacheObject<T> : Singleton<CacheObject<T>>
    {
        public List<T> CacheList = new List<T>();

        public void ReleaseCache()
        {
            CacheList.Clear();
        }
    }
}
