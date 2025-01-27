using System.Collections.Generic;

namespace Config
{
    abstract class CacheObject<T>
    {
        public List<T> CacheList = new List<T>();

        public void ReleaseCache()
        {
            CacheList.Clear();
        }
    }
}
