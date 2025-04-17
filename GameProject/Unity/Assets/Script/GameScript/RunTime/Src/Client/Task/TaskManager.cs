using System;
using System.Collections.Generic; 
using Cysharp.Threading.Tasks;
using DebugTool;
using EventSystem;
using SingleTool;

namespace MyGame
{
    public class TaskManager : Singleton<TaskManager>
    {
        private List<Type> progresses = new List<Type>();
        private Dictionary<Type, ITask> progressDict = new Dictionary<Type, ITask>(); 
        private Type curProgressType; //当前流程    
        
        public void Init()
        {
            DLogger.Log("==============>Init ProgressManager");  
            //监听对应事件
            this.Subscribe<Type>(TaskEvent.TaskChange, ChangeProgress); 
            this.Subscribe<Type>(TaskEvent.TaskSetCurTask,SetCurProgressType);
            this.Subscribe<Type,ITask>(TaskEvent.TaskAddTask,AddProgress); 
            this.Subscribe(TaskEvent.TaskLunch,Lunch); 
        } 

        private void AddProgress(Type type,ITask task)
        {
            progresses.Add(type);
            progressDict.Add(type, task); 
        } 

        private void SetCurProgressType(Type progressType)
        {
            curProgressType = progressType;
        }  

        private void ChangeProgress(Type progressType)
        {
            if (progressType != null)
            { 
                if (progressDict.TryGetValue(progressType, out ITask progress))
                {  
                    curProgressType = progressType; 
                    progress.Run().Forget();
                } 
            }
        }

        private void Lunch()
        {
            DLogger.Log("==============>Game Start!!!!");
            if (curProgressType != null)
            {
                if (progressDict.TryGetValue(curProgressType, out ITask progress))
                {
                    progress.Run().Forget();
                }
            }
        }
    }
}