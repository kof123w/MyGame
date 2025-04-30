using System;
using System.Collections.Generic;
using BEPUphysics.Entities;
using EventSystem;
using FixedMath;

namespace MyGame
{
    public class RolePerformer : Fix64RoleBody
    {
        public long PlayerRoleID { get; set; }

        protected override void InitEntityShapeParam()
        {
            // 禁用旋转（防止角色摔倒）
            EntityShape.LocalInertiaTensorInverse = new FPMatrix3x3();
            EntityShape.LinearDamping = 0.1f;
            base.InitEntityShapeParam();
        }

        public override void SyncWorld()
        {
            base.SyncWorld();
            var worldPosition = EntityShape.Position;
            var worldOrientation = EntityShape.Orientation; 
            var worldPos = MathConvertor.Fix3ToVector3(ref worldPosition);
            var worldQri = MathConvertor.FixQToQuaternion(ref worldOrientation); 
            GameEvent.Push(VisualSignal.VisualSignal_SetVisualPosition,PlayerRoleID,worldPos,worldQri);
        }

        #region 状态机相关
        private readonly Dictionary<int, Tuple<Action<int>, Action<int>, Action<float>>> actions = new();
        private readonly List<ValueTuple<int, int, int>> transitions = new();
        public int CurrentState { get; private set; }
        private Action<float> beforeUpdateCallBack;
        private Action<float> afterUpdateCallBack; 
        
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