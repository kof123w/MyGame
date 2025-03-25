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
            if (m_ObjectPool.TryGetValue(type, out var pool))
            {
                return pool.Pop();
            } 
            var obj = Activator.CreateInstance(type) as IMemoryPool;
            return obj;
        }

        public bool DestroyRecycle(Type type, IMemoryPool entity)
        {
            if (m_ObjectPool.TryGetValue(type, out var pool))
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