using System;

namespace MyGame
{
    public interface IFsm{
        int CurrentState {get;}
        //添加对应的状态码，设置进入、退出、更新状态的时候调用的回调。
        bool AddState(int state,Action<int> onEnter,Action<int> onExit,Action<float> onUpdate);
        //删除掉这个状态
        bool RemoveState(int state);
        //更新函数
        void Update(float time);
        //添加从状态from到to使用什么触发码
        bool AddTransition(int from,int to,int triggerCode);
        //触发一下对应时间
        bool TriggerEvent(int eventCode);
        //状态切换
        bool SwitchToState(int stateId,bool forceSwitch);

        public void InitFsm(Action<float> beforeUpdate, Action<float> afterUpdate, int defaultState = -1);
    }
}