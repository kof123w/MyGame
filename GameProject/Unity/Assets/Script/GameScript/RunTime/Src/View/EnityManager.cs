using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace MyGame
{
    public class EntityManager
    {
        private List<Entity> m_Entities; 
        private Dictionary<long, ComponentData> m_ComponentDataDict;  //查找使用
        private EntityIDGenerator m_EntityIDGenerator;
        private ComponentIDGenerator m_ComponentIDGenerator;
        private Dictionary<Type,List<ComponentSystem>> m_ComponentSystems;  
        
        public EntityManager()
        {
            m_Entities = new List<Entity>(); 
            m_ComponentDataDict = new Dictionary<long, ComponentData>();
            m_EntityIDGenerator = new EntityIDGenerator();
            m_ComponentSystems = new Dictionary<Type, List<ComponentSystem>>();
            m_ComponentIDGenerator = new ComponentIDGenerator();
            
             
        }

        public T InstantiateEntity<T>() where T:Entity,new ()
        {
            long genId = m_EntityIDGenerator.GenerateID();
            IMemoryPool obj = Pool.Instance.CreateFromPool(typeof(T));
            T t = obj as T;
            m_Entities.Add(t);
            t?.SetId(genId);
            return t;
        }

        public bool DestroyEntity<T>(T entity) where T : Entity, new()
        {
            var list = entity.GetComponents();
            if (list is { Count: > 0 })
            {
                for (int i = list.Count-1; i >= 0; i--)
                {
                    long componentId = list[i];
                    if (m_ComponentDataDict.ContainsKey(componentId))
                    { 
                        m_ComponentDataDict.Remove(componentId);
                    }
                }
            }
            entity.ClearComponents();
            var result = Pool.Instance.DestroyRecycle(typeof(T),entity);
            return result;
        }
        
        public void EntityUpdate()
        {
            foreach (var componentDataPair in m_ComponentDataDict)
            {
                var componentData = componentDataPair.Value;
                if (m_ComponentSystems.TryGetValue(componentData.GetType(), out var componentSystemList))
                {
                    for (int i = 0; i < componentSystemList.Count; i++) {
                        IUpdate update = componentSystemList[i] as IUpdate;
                        if (update != null)
                        { 
                            update.Update(ref componentData);
                        }
                    }
                }
            }
        }
        private void ComponentDestroy(ComponentData componentData)
        {
            if (m_ComponentSystems.TryGetValue(componentData.GetType(), out var systemList))
            {
                for (int i = 0; i < systemList.Count; i++) {
                    IOnDestroy destroy = systemList[i] as IOnDestroy;
                    if (destroy != null)
                    {
                        destroy.OnDestroy(ref componentData);
                    }
                }
            }

            Pool.Instance.DestroyRecycle(componentData.GetType(),componentData);
        }

        private void ComponentStart(ComponentData componentData)
        {
            if (m_ComponentSystems.TryGetValue(componentData.GetType(), out var systemList))
            {
                for (int i = 0; i < systemList.Count; i++)
                {
                    IStart start = systemList[i] as IStart;
                    if (start != null)
                    {
                        start.Start(ref componentData);
                    }
                } 
            }
        }
    }

    class EntityIDGenerator
    {
        private volatile int m_EneityBaseGenID = 0;

        public int GenerateID()
        {
            // ReSharper disable once NonAtomicCompoundOperator
            m_EneityBaseGenID++;
            return m_EneityBaseGenID;
        }
    }

    class ComponentIDGenerator
    {
        private volatile int m_ComponentBaseGenID = 0;

        public int GenerateID()
        {
            // ReSharper disable once NonAtomicCompoundOperator
            m_ComponentBaseGenID++;
            return m_ComponentBaseGenID;
        }
    }

    public static class EntityManagerExtension
    {
        public static bool AddComponent<T>() where T : ComponentData
        {
            return true;
        }
    }


}