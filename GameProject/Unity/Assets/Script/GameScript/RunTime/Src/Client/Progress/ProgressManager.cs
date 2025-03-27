using System;
using System.Collections.Generic; 
namespace MyGame
{
    public class ProgressManager : Singleton<ProgressManager>
    {
        private List<Type> progresses = new List<Type>();
        private Dictionary<Type, IProgress> progressDict = new Dictionary<Type, IProgress>();
        private Dictionary<Type, bool> finishDict = new Dictionary<Type, bool>();
        private Type curProgressType; //当前流程   
        private List<IProgress> needCheckProgress = new List<IProgress>();

        public void Init()
        {
            DLogger.Log("==============>Init ProgressManager");  
            //监听对应事件
            this.Subscribe<Type>(ProgressEvent.ProgressChange, ChangeProgress);
            this.Subscribe<Type>(ProgressEvent.ProgressMakeDirty, ProgressMakeDirty);
            this.Subscribe<Type>(ProgressEvent.ProgressFinishDirt, ProgressFinishMake);  
            this.Subscribe<Type>(ProgressEvent.ProgressSetCurProgress,SetCurProgressType);
            this.Subscribe<Type,IProgress>(ProgressEvent.ProgressAddProgress,AddProgress);
            this.Subscribe<IProgress>(ProgressEvent.ProgressAddNeeCheckProgress,AddNeedCheckProgress); 
            this.Subscribe(ProgressEvent.ProgressLunch,Lunch); 
        }

        public void Update()
        {
            foreach (IProgress progress in needCheckProgress)
            {
                progress.Check();
            }
        }

        private void AddProgress(Type type,IProgress progress)
        {
            progresses.Add(type);
            progressDict.Add(type, progress); 
        }
        
        private void AddNeedCheckProgress(IProgress progress)
        {
            needCheckProgress.Add(progress);
        }

        private void SetCurProgressType(Type progressType)
        {
            curProgressType = progressType;
        }

        private void ProgressFinishMake(Type progressType)
        {
            if (progressType != null)
            {
                finishDict[progressType] = true;
            }
        }

        private void ProgressMakeDirty(Type progressType)
        {
            if (progressType != null)
            {
                if (finishDict.ContainsKey(progressType))
                {
                    finishDict[progressType] = false;
                }
            }
        }

        private void ChangeProgress(Type progressType)
        {
            if (progressType != null)
            {
                if (progressDict.TryGetValue(progressType, out IProgress progress))
                {
                    progress.Run();
                    curProgressType = progressType;
                }
            }
        }

        private void Lunch()
        {
            DLogger.Log("==============>Game Start!!!!");
            if (curProgressType != null)
            {
                if (progressDict.TryGetValue(curProgressType, out IProgress progress))
                {
                    progress.Run();
                }
            }
        }
    }
}