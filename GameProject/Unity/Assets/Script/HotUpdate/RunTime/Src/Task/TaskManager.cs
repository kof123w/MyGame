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
        private List<Type> tasks = new List<Type>();
        private Dictionary<Type, ITask> taskDict = new Dictionary<Type, ITask>(); 
        private Type curTaskType; //当前流程    
        
        public void Init()
        {
            DLogger.Log("==============>Init ProgressManager");  
            //监听对应事件
            this.Subscribe<Type>(TaskEvent.TaskChange, ChangeTask); 
            this.Subscribe<Type>(TaskEvent.TaskSetCurTask,SetCurTaskType);
            this.Subscribe<Type,ITask>(TaskEvent.TaskAddTask,AddTask); 
            this.Subscribe(TaskEvent.TaskLunch,Lunch); 
        } 

        private void AddTask(Type type,ITask task)
        {
            tasks.Add(type);
            taskDict.Add(type, task); 
        } 

        private void SetCurTaskType(Type taskType)
        {
            curTaskType = taskType;
        }  

        private void ChangeTask(Type taskType)
        {
            if (taskType != null)
            { 
                if (taskDict.TryGetValue(taskType, out ITask progress))
                {  
                    curTaskType = taskType; 
                    progress.Run().Forget();
                } 
            }
        }

        private void Lunch()
        {
            DLogger.Log("==============>Game Start!!!!");
            if (curTaskType != null)
            {
                if (taskDict.TryGetValue(curTaskType, out ITask task))
                {
                    task.Run().Forget();
                }
            }
        }
    }
}