using System;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
    public class Pool : Singleton<Pool>
    {
        private Dictionary<Type,Stack<IMemoryPool>> m_ObjectPool = new Dictionary<Type, Stack<IMemoryPool>>();

        public IMemoryPool CreateFromPool(Type type)
        {
            Stack<IMemoryPool> pool;
            if (m_ObjectPool.TryGetValue(type, out pool))
            {
                return pool.Pop();
            } 
            IMemoryPool obj = Activator.CreateInstance(type) as IMemoryPool;
            return obj;
        }

        public bool DestroyRecycle(Type type, IMemoryPool entity)
        {
            Stack<IMemoryPool> pool;
            if (m_ObjectPool.TryGetValue(type, out pool))
            {
                pool.Push(entity); 
            }
            else
            {
                pool = new Stack<IMemoryPool>();
                pool.Push(entity);
                m_ObjectPool.Add(type, pool);
            }

            return true;
        }
    }
}