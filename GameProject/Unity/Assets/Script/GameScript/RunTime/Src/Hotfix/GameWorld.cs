using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine.UI;

namespace MyGame
{
    public sealed class GameWorld : Singleton<GameWorld>,IEcsManager
    {
        //创建对象时使用，唯一的ID
        private long m_curEntityId = 0; 
        
        //创建的对象时使用，唯一id
        private long m_curComponentId = 0;
        
        //创建的系统唯一id
        private long m_curSystemId = 0;
            
        //当前世界下的所有实体
        private Dictionary<long, Entity> m_dictEntity = new Dictionary<long, Entity> ();
        
        //Entity 索引的 Component
        private Dictionary<long, Dictionary<long, ComponentData>> m_dictComponent =  new  Dictionary<long, Dictionary<long, ComponentData>>();
        
        //System的函数,只要是用来执行某个组件的生命周期的
        private Dictionary<long, ISystem> m_dictSystem = new Dictionary<long, ISystem> ();
  
        //初始化函数
        public void Init()
        {
            DLogger.Log("=====Init Game World!!!!=====");
            
            m_curEntityId = 0;
            m_curComponentId = 0;
            m_curSystemId = 0;
            m_dictEntity.Clear();
            
            //注册事件
           this.Subscribe<long,long>(GameWorldConst.GameWorldAwakeEventId,Awake); 
           this.Subscribe<long,long>(GameWorldConst.GameWorldEnableEventId,OnEnable);
           this.Subscribe<long,long>(GameWorldConst.GameWorldDisableEventId,Disable);
           this.Subscribe<long,long>(GameWorldConst.GameWorldStartEventId,Start);
           this.Subscribe<long,long>(GameWorldConst.GameWorldDestoryEventId,Destroy);
        }

        //所有实体的生命周期都从这里获取
        public void Update()
        {
            using var itor = m_dictSystem.GetEnumerator();
            while (itor.MoveNext())
            {
                if (itor.Current.Value is IUpdate updateObj) 
                {
                    updateObj.Update();
                }
            }
        }

        public ISystem GetSystem(long entityId,long componentId)
        {
            Dictionary<long, ComponentData> tmp; 
            if (m_dictComponent.TryGetValue(entityId,out tmp))
            {
                long sysId = 0;
                ComponentData component;
                if (tmp.TryGetValue(componentId,out component))
                {
                    sysId = component.SystemId;
                    ISystem system;
                    if (m_dictSystem.TryGetValue(sysId,out system))
                    {
                        return system;
                    }
                }
            }

            return null;
        }

        public ISystem GetSystem(long systemId)
        {
            ISystem system;
            if (m_dictSystem.TryGetValue(systemId,out system))
            {
                return system;
            }

            return null;
        }

        public void Awake(long entityId,long componentId)
        {
            ISystem systemObj = GetSystem(entityId,componentId);
            IAwake awake = systemObj as IAwake;
            if (awake != null)
            {
                awake.Awake();
            }
        }

        public void OnEnable(long entityId,long componentId)
        {
            ISystem systemObj = GetSystem(entityId,componentId);
            IOnEnable onEnable = systemObj as IOnEnable;
            if (onEnable != null)
            {
                onEnable.OnEnable();
            }
        }

        public void Disable(long entityId,long componentId)
        {
            ISystem systemObj = GetSystem(entityId,componentId);
            IOnDisable disable = systemObj as IOnDisable;
            if (disable != null)
            {
                disable.OnDisable();
            }
        }
        
        public void Start(long entityId,long componentId)
        {
            ISystem systemObj = GetSystem(entityId,componentId);
            IStart Start = systemObj as IStart;
            if (Start != null)
            {
                Start.Start();
            }
        }

        public void Destroy(long entityId,long componentId)
        {
            ISystem systemObj = GetSystem(entityId,componentId);
            IOnDestroy destroy = systemObj as IOnDestroy;
            if (destroy != null)
            {
                destroy.OnDestroy();
            }
        }


        // todo 后续使用内存池进行管理
        public T Instacing<T>() where T : Entity,new ()
        {
            Entity entity = new T();
            entity.EntityId = GeneratorEntityId();
            m_dictEntity.Add(entity.EntityId,entity);
            return entity as T;
        }

        public T AddComponent<T>(Entity entity) where T : ComponentData,new ()
        {
            if (m_dictEntity.ContainsKey(entity.EntityId))
            {
                long id = typeof(T).ToString().StringToHash();
                Dictionary<long, ComponentData> componentDict;
                if (m_dictComponent.TryGetValue(entity.EntityId,out componentDict))
                {
                    ComponentData componentData;
                    if (componentDict.TryGetValue(id,out componentData))
                    {
                        return null;
                    }
                    else
                    {
                        componentData = new T();
                        ISystem system = CreateSystem<T>();
                        componentData.SystemId = system.SystemId;
                        componentData.EntityId = entity.EntityId;
                        componentData.ComponentId = id; 
                        
                        system.EntityId = entity.EntityId;
                        system.ComponentId = id; 
                        return componentData as T;
                    }
                }
                else
                {
                    componentDict = new Dictionary<long, ComponentData>();
                    ComponentData componentData = new T();
                    componentDict.Add(id,componentData);
                    m_dictComponent.Add(entity.EntityId,componentDict);
                    ISystem system = CreateSystem<T>();
                    componentData.SystemId = system.SystemId;
                    componentData.EntityId = entity.EntityId;
                    componentData.ComponentId = id; 
                        
                    system.EntityId = entity.EntityId;
                    system.ComponentId = id; 
                    return componentData as T;
                }
            }

            return null;
        }

        public void RemoveComponent<T>(Entity entity)
        {
            Dictionary<long, ComponentData> componentDict;
            if (m_dictComponent.TryGetValue(entity.EntityId,out componentDict))
            {
                long id = typeof(T).ToString().StringToHash();
                ComponentData componentData;
                bool isFind = false;
                if (componentDict.TryGetValue(id,out componentData))
                {
                    isFind = true;
                    Instance.Push(GameWorldConst.GameWorldDisableEventId,entity.EntityId,componentData.ComponentId);
                    Instance.Push(GameWorldConst.GameWorldDestoryEventId,entity.EntityId,componentData.ComponentId);
                }

                if (isFind)
                {
                    m_dictSystem.Remove(componentData.SystemId);
                    componentDict.Remove(id);
                }
            }
           
        }

        [CanBeNull]
        private ISystem CreateSystem<T>() where T : ComponentData
        {
            //创建System
            Type type = typeof(T);
            var attrs = Attribute.GetCustomAttributes(type,typeof(System));
            System sysAttr = attrs[0] as System;
            if (sysAttr != null)
            {
                DLogger.Log(sysAttr.GetType()+"=============");
               // sysAttr.GetType().Assembly.CreateInstance(sysAttr.GetType().);
               ISystem system = Assembly.GetAssembly(sysAttr.GetType()).CreateInstance(sysAttr.GetType().ToString()) as ISystem;
               system.SystemId = GeneratorSystemId();
               m_dictSystem.Add(system.SystemId,system);
               return system;
            }

            return null;
        }

        public bool DestroyEntity(Entity entity)
        {
            var entityId = entity.EntityId;
            
            //先处理下生命周期要通知的东西
            Dictionary<long, ComponentData> componentDict;
            bool isFind = false;
            if (m_dictComponent.TryGetValue(entityId,out componentDict))
            {
                var itor = componentDict.GetEnumerator();
                while (itor.MoveNext())
                {
                    var val = itor.Current.Value;
                    Instance.Push(GameWorldConst.GameWorldDisableEventId,entityId,val.ComponentId);
                }
                
                while (itor.MoveNext())
                {
                    var val = itor.Current.Value;
                    Instance.Push(GameWorldConst.GameWorldDestoryEventId,entityId,val.ComponentId);
                }

                isFind = true;
            }

            m_dictEntity.Remove(entityId);
            //清理下对象
            if (isFind)
            {
                var itor = componentDict.GetEnumerator();
                while (itor.MoveNext())
                {
                    var val = itor.Current.Value;
                    m_dictSystem.Remove(val.SystemId);
                }
               
                m_dictComponent.Remove(entityId);
            }

            return true;
        }

        public long GeneratorEntityId()
        {
            m_curEntityId++;
            return m_curEntityId;
        }

        public long GeneratorComponentId()
        {
            m_curComponentId++;
            return m_curComponentId;
        }

        public long GeneratorSystemId()
        {
            m_curSystemId++;
            return m_curSystemId;
        }
    } 
    
    //直接用做成扩展方法的东西
    public static class GameEntity 
    {
        public static T AddComponent<T>(this Entity entity,bool isEnable = true) where T : ComponentData,new ()
        {
            ComponentData componentData = GameWorld.Instance.AddComponent<T>(entity);
            GameWorld.Instance.Push(GameWorldConst.GameWorldAwakeEventId);
            if (isEnable)
            {
                componentData.Enable = true; 
            }
            return componentData as T;
        }

        public static void RemoveComponent<T>(this Entity entity)
        {
            entity.RemoveComponent<T>();
        }


        public static T Instacing<T>() where T : Entity,new ()
        {
            return GameWorld.Instance.Instacing<T>();
        }

        public static bool Destroy(Entity entity)
        {
            return GameWorld.Instance.DestroyEntity(entity);
        }
    } 
}
