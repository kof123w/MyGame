using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace MyGame
{
    public class Pool : Singleton<Pool>
    {
        private Dictionary<Type,List<IMemoryPool>> m_ObjectPool = new Dictionary<Type, List<IMemoryPool>>();

        public static T Malloc<T>() where T : class, IMemoryPool, new()
        {
            if (Instance == null)
            {
                return default(T);
            }

            T t = Instance.CreateFromPool(typeof(T)) as T;
            return t;
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

            if (Instance.m_ObjectPool.ContainsKey(typeof(T)))
            {
                Instance.m_ObjectPool.Remove(typeof(T));
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

            if (Instance.m_ObjectPool!=null)
            {
                Instance.m_ObjectPool.Clear();
            }
        }

        public IMemoryPool CreateFromPool(Type type)
        {
            IMemoryPool obj;
            if (m_ObjectPool.TryGetValue(type, out var pool))
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
            if (m_ObjectPool.TryGetValue(type, out var pool))
            {
                pool.Add(entity); 
            }
            else
            {
                pool = new List<IMemoryPool>(20);
                pool.Add(entity);
                m_ObjectPool.Add(type, pool);
            }
            return true;
        }
    }
}