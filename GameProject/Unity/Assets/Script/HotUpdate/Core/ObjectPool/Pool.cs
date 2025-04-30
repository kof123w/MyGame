using System;
using System.Collections.Generic;
using MyGame;
using SingleTool;

namespace ObjectPool
{
    public class Pool : Singleton<Pool>
    {
        private Dictionary<Type,List<IMemoryPool>> objectPool = new Dictionary<Type, List<IMemoryPool>>();

        public static T Malloc<T>() where T : class, IMemoryPool, new()
        {
            if (Instance == null)
            {
                return default(T);
            }

            T t = Instance.CreateFromPool(typeof(T)) as T;
            return t;
        }

        public static void Free<T>(List<T> objects) where T : class, IMemoryPool, new()
        {
            if (objects == null)
            {
                return;
            }

            for (int i = 0; i < objects.Count; i++)
            {
                objects[i].OnDestroy();
                Free(objects[i]);
            }
            
            objects.Clear();
        }
        
        public static void Free<T>(T[] objects) where T : class, IMemoryPool, new()
        {
            if (objects == null)
            {
                return;
            }

            for (int i = 0; i < objects.Length; i++)
            {
                objects[i].OnDestroy();
                Free(objects[i]);
            }
        }

        public static void Free<T>(T obj) where T : class, IMemoryPool, new()
        {
            if (Instance == null)
            {
                return;
            }

            Instance.DestroyRecycle(typeof(T), obj);
        }

        /// <summary>
        /// 清理一下对应类型到内存池
        /// </summary>
        public static void ClearMemoryByType<T>()
        {
            if (Instance == null)
            {
                return;
            }

            if (Instance.objectPool.ContainsKey(typeof(T)))
            {
                Instance.objectPool.Remove(typeof(T));
            }
        }

        /// <summary>
        /// 清理一下内存池
        /// </summary> 
        public static void ClearMemory()
        {
            if (Instance == null)
            {
                return;
            }

            if (Instance.objectPool!=null)
            {
                Instance.objectPool.Clear();
            }
        }

        public IMemoryPool CreateFromPool(Type type)
        {
            IMemoryPool obj;
            if (objectPool.TryGetValue(type, out var pool))
            {
                if (pool.Count > 0)
                {
                    var index = pool.Count - 1;
                    obj = pool[index];
                    pool.RemoveAt(index);
                    return obj;
                } 
            } 
            obj = Activator.CreateInstance(type) as IMemoryPool;
            return obj;
        }

        public bool DestroyRecycle(Type type, IMemoryPool entity)
        {
            if (objectPool.TryGetValue(type, out var pool))
            {
                pool.Add(entity); 
            }
            else
            {
                pool = new List<IMemoryPool>(20);
                pool.Add(entity);
                objectPool.Add(type, pool);
            }
            return true;
        }
    }
}