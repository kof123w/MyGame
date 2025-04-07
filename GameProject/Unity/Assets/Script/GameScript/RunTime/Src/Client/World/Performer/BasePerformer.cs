using System;
using System.Collections.Generic;
using Config;
using MyGame;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace MyGame
{
    //场景对象，所有直接继承一个状态机
    public class BasePerformer : AssetObject,IFsm,IMemoryPool
    {
        protected GameObject posGameObj;
        protected Transform posTrans;
        private readonly Dictionary<int, Tuple<Action<int>, Action<int>, Action<float>>> actions = new();
        private List<ValueTuple<int, int, int>> transitions = new();
        public int CurrentState { get; private set; }
        private Action<float> beforeUpdateCallBack;
        private Action<float> afterUpdateCallBack;
        private RoleResourceConfig roleResourceConfig;

        public BasePerformer()
        {
            posGameObj = ObjectPool.MallocEmptyNode();
            posTrans = posGameObj.transform;
            posTrans.SetParent(GameWorld.GetGameWorldTransform());
        }

        public async UniTaskVoid UnLoadResources()
        {
            await UnloadResource();
        } 
        
        public void SetConfigID(int id)
        {
            roleResourceConfig = ResourceConfigManager.Instance.GetRoleResourceConfig(id); 
        } 
        
        public async UniTask LoadActor()
        {
            await LoadAsset(roleResourceConfig.AssetPath);
        }

        public void SetWorldPos(Vector3 pos)
        {
            posTrans.position = pos;
        }

        public void SetGameObjectName(string gameObjectName)
        {
            posGameObj.name = gameObjectName;
        } 

        #region 状态机相关
        public void InitFsm(Action<float> beforeUpdate, Action<float> afterUpdate, int defaultState = -1)
        {
            CurrentState = defaultState;
            beforeUpdateCallBack = beforeUpdate;
            afterUpdateCallBack = afterUpdate;
        }

        public bool AddState(int state, Action<int> onEnter, Action<int> onExit, Action<float> onUpdate)
        {
            if (actions.ContainsKey(state))
            {
                throw new Exception($"不能重复添加状态{state}行为");
            }

            actions.Add(state, new Tuple<Action<int>, Action<int>, Action<float>>(onEnter, onExit, onUpdate));
            return true;
        }

        public bool RemoveState(int state)
        {
            if (!actions.ContainsKey(state))
            {
                return false;
            }

            return actions.Remove(state);
        }

        public bool AddTransition(int from, int to, int triggerCode)
        {
            if (!actions.ContainsKey(from) || !actions.ContainsKey(to))
            {
                return false;
            }

            transitions.Add((from, to, triggerCode));
            return true;
        }

        public bool TriggerEvent(int eventCode)
        {
            foreach (var transition in transitions)
            {
                if (transition.Item1 == CurrentState && transition.Item3 == eventCode)
                {
                    SwitchToState(transition.Item2);
                    return true;
                }
            }

            return false;
        }

        public bool AddState(int state)
        {
            if (actions.ContainsKey(state))
            {
                actions.Remove(state);
                return true;
            }

            return false;
        }

        public bool SwitchToState(int stateId, bool forceSwitch = false)
        {
            bool hasState = actions.ContainsKey(stateId);
            if (!hasState) return false;

            bool stateChanged = stateId != CurrentState;
            if (!stateChanged && !forceSwitch) return false;

            if (stateChanged)
            {
                if (actions.TryGetValue(CurrentState, out var oldActions))
                {
                    oldActions.Item2?.Invoke(stateId);
                }
            }

            var oldStateId = CurrentState;
            var newActions = actions[stateId];
            CurrentState = stateId;
            newActions.Item1?.Invoke(oldStateId);
            return true;
        }

        public void Update(float time)
        {
            if (actions.TryGetValue(CurrentState, out var action))
            {
                beforeUpdateCallBack?.Invoke(time);
                action.Item3?.Invoke(time);
                afterUpdateCallBack?.Invoke(time);
            }
        }
        
        #endregion 状态机相关
    }
}
