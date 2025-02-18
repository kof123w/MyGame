using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace MyGame
{
    public class EntityManager
    {
        private readonly Dictionary<long,Entity> _mEntities; 
        private readonly Dictionary<long, Dictionary<long,ComponentData>> _mComponentDataDict;  //查找使用
        private readonly EntityIDGenerator _mEntityIDGenerator;
        private readonly ComponentIDGenerator _mComponentIDGenerator;
        private readonly Dictionary<Type,ComponentSystem> _mComponentSystems;  
        
        public EntityManager()
        {
            _mEntities = new Dictionary<long, Entity>();
            _mComponentDataDict = new Dictionary<long, Dictionary<long, ComponentData>>();
            _mEntityIDGenerator = new EntityIDGenerator();
            _mComponentSystems = new Dictionary<Type, ComponentSystem>();
            _mComponentIDGenerator = new ComponentIDGenerator();
            
            DLogger.Log("=============EntityManager::Init Ecs System===============");
            Type baseType = typeof(ComponentSystem);
            var assembly = Assembly.GetExecutingAssembly();
            var types = assembly.GetTypes();

            var componentSystemTypes = types.Where(t=>baseType.IsAssignableFrom(t));
            foreach (var systemType in componentSystemTypes)
            {
                if (systemType != baseType)
                {
                    SystemOfComponent systemOfComponent = systemType.GetCustomAttribute<SystemOfComponent>(true);
                    if (systemOfComponent != null)
                    {
                        Type componentType = systemOfComponent.Type;
                        if (!_mComponentSystems.ContainsKey(componentType))
                        {  
                            _mComponentSystems.Add(componentType, Activator.CreateInstance(systemType) as ComponentSystem);
                        } 
                    }
                }
            }
        }

        public EntityManager(Dictionary<long, Entity> mEntities, Dictionary<long, Dictionary<long, ComponentData>> mComponentDataDict)
        {
            _mEntities = mEntities;
            _mComponentDataDict = mComponentDataDict;
        }

        public T InstantiateEntity<T>() where T:Entity,new ()
        {
            long genId = _mEntityIDGenerator.GenerateID();
            IMemoryPool obj = Pool.Instance.CreateFromPool(typeof(T));
            T t = obj as T;
            _mEntities.Add(genId,t);
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
                    if (_mComponentDataDict.ContainsKey(componentId))
                    { 
                        _mComponentDataDict.Remove(componentId);
                    }
                }
            }
            entity.ClearComponents();
            var result = Pool.Instance.DestroyRecycle(typeof(T),entity);
            return result;
        }
        
        public void EntityUpdate()
        {
            foreach (var componentDataDictPair in _mComponentDataDict)
            {
                var componentDataDict = componentDataDictPair.Value;
                foreach (var componentDataPair in componentDataDict)
                {
                    ComponentData componentData = componentDataPair.Value;
                    if (_mComponentSystems.TryGetValue(componentData.GetType(), out var system))
                    {
                        var update = system as IUpdate;
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
            if (_mComponentSystems.TryGetValue(componentData.GetType(), out var system))
            {
                IOnDestroy destroy = system as IOnDestroy;
                if (destroy != null)
                {
                    destroy.OnDestroy(ref componentData);
                }
            }

            Pool.Instance.DestroyRecycle(componentData.GetType(),componentData);
        }
        private void ComponentStart(ComponentData componentData)
        {
            if (_mComponentSystems.TryGetValue(componentData.GetType(), out var system))
            {
                IStart start = system as IStart;
                if (start != null)
                {
                    start.Start(ref componentData);
                }
            }
        }

        public T AddComponent<T>(Entity entity) where T : ComponentData, new()
        {
            if (entity != null)
            {
                IMemoryPool obj = Pool.Instance.CreateFromPool(typeof(T));
                T t = obj as T;
                int componentId = _mComponentIDGenerator.GenerateID();
                t.SetId(componentId);
                t.SetEntityId(entity.BaseID);
                bool isSuccess = entity.AddComponent(componentId);
                if (isSuccess)
                {
                    ComponentStart(t);
                    return t;
                }
            }

            return null;
        }

        public ComponentData AddComponentByType(Entity entity,Type type)
        {
            if (entity != null)
            {
                IMemoryPool obj = Pool.Instance.CreateFromPool(type);
                ComponentData t = obj as ComponentData;
                int componentId = _mComponentIDGenerator.GenerateID();
                if (t != null)
                {
                    t.SetId(componentId);
                    t.SetEntityId(entity.BaseID);
                    bool isSuccess = entity.AddComponent(componentId);
                    if (isSuccess)
                    {
                        ComponentStart(t);
                        return t;
                    }
                }
            }
            return null;
        }

        public bool RemoveComponent<T>(Entity entity,long componentId) where T : ComponentData, new()
        {
            if (entity != null) {
                bool isSuccess = entity.RemoveComponent(componentId);
                if (isSuccess) {
                    if (_mComponentDataDict.ContainsKey(componentId)) {
                        T t = _mComponentDataDict[componentId] as T;
                        if (t != null)
                        {
                            ComponentDestroy(t);
                        }
                        _mComponentDataDict.Remove(componentId);
                    }
                }
            } 
            return false;
        }

        public bool RemoveComponentByType(Entity entity, Type type)
        {
            if (entity != null)
            {
                if (_mComponentDataDict.TryGetValue(entity.BaseID, out Dictionary<long, ComponentData> componentDataDict))
                {
                    long componentId = 0;
                    bool isFind = false;
                    ComponentData findComponentData = null;
                    foreach (var componentDataPair in componentDataDict)
                    {
                        ComponentData componentData = componentDataPair.Value;
                        if (componentData.GetType() == type)
                        {
                            isFind = true;
                            componentId = componentDataPair.Key;
                            findComponentData = componentData;
                            break;
                        }
                    }

                    if (isFind)
                    {
                        ComponentDestroy(findComponentData);
                    }
                    entity.RemoveComponent(componentId);
                    componentDataDict.Remove(componentId);
                }
            } 
            return false;
        }

        public ComponentSystem GetComponentSystem(Type componentType)
        {
            return _mComponentSystems.GetValueOrDefault(componentType);
        }
    }

    class EntityIDGenerator
    {
        private volatile int _mEntityBaseGenID = 0;

        public int GenerateID()
        {
            // ReSharper disable once NonAtomicCompoundOperator
            _mEntityBaseGenID++;
            return _mEntityBaseGenID;
        }
    }

    class ComponentIDGenerator
    {
        private volatile int _mComponentBaseGenID = 0;

        public int GenerateID()
        {
            // ReSharper disable once NonAtomicCompoundOperator
            _mComponentBaseGenID++;
            return _mComponentBaseGenID;
        }
    }

    public static class EntityManagerExtension
    {
        public static T AddComponent<T>(this Entity entity) where T : ComponentData,new ()
        {
            GameWorld gameWorld = GameWorld.Instance;
            return gameWorld.EntityManager.AddComponent<T>(entity);
        }

        public static ComponentData AddComponentByType(this Entity entity, Type type)
        {
            GameWorld gameWorld = GameWorld.Instance;
            return gameWorld.EntityManager.AddComponentByType(entity, type);
        }

        public static bool RemoveComponent<T>(Entity entity,long componentId) where T : ComponentData, new()
        {
            GameWorld gameWorld = GameWorld.Instance;
            return gameWorld.EntityManager.RemoveComponent<T>(entity, componentId);
        }

        public static bool RemoveComponentByType(this Entity entity, Type type)
        {
            GameWorld gameWorld = GameWorld.Instance;
            return gameWorld.EntityManager.RemoveComponentByType(entity, type);
        }
    }
}