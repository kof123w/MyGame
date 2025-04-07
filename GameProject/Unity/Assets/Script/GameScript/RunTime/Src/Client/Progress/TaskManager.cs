using System;
using System.Collections.Generic; 
using Cysharp.Threading.Tasks;

namespace MyGame
{
    public class TaskManager : Singleton<TaskManager>
    {
        private List<Type> progresses = new List<Type>();
        private Dictionary<Type, ITask> progressDict = new Dictionary<Type, ITask>(); 
        private Type curProgressType; //当前流程   
        private HashSet<Type> needCheckProgress = new HashSet<Type>(); 
        
        public void Init()
        {
            DLogger.Log("==============>Init ProgressManager");  
            //监听对应事件
            this.Subscribe<Type>(TaskEvent.TaskChange, ChangeProgress); 
            this.Subscribe<Type>(TaskEvent.TaskSetCurProgress,SetCurProgressType);
            this.Subscribe<Type,ITask>(TaskEvent.TaskAddProgress,AddProgress);
            this.Subscribe<Type>(TaskEvent.TaskAddNeeCheckProgress,AddNeedCheckProgress); 
            this.Subscribe(TaskEvent.TaskLunch,Lunch); 
        } 

        private void AddProgress(Type type,ITask task)
        {
            progresses.Add(type);
            progressDict.Add(type, task); 
        }
        
        private void AddNeedCheckProgress(Type type)
        {
            needCheckProgress.Add(type);
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