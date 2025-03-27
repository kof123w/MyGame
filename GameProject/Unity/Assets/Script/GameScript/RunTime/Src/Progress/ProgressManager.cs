using System;
using System.Collections.Generic;
using System.Reflection;
using MyGame;
using Assembly = System.Reflection.Assembly;

namespace MyGame
{
    public class ProgressManager : Singleton<ProgressManager>
{ 
    private List<Type> _mProgresses = new List<Type>();   
    private Dictionary<Type, IProgress> _mProgressDict = new Dictionary<Type, IProgress>();
    private Dictionary<Type,bool> _mFinishDict = new Dictionary<Type, bool>();
    private Type _curProgressType;   //当前流程   
    private List<IProgress> _needCheckProgress = new List<IProgress>();
    
    public void Init()
    {
         DLogger.Log("=================Init ProgressManager=================");
         var types = Assembly.GetExecutingAssembly().GetTypes();
         var interfaceType = typeof(IProgress);
         for (int i = 0; i < types.Length; i++) {
             var type = types[i];
             var interfaces = type.GetInterfaces();
             bool isInit = false; 
             bool needCheck = false;
             for (int j = 0; j < interfaces.Length; j++) {
                 if (interfaces[j].Name.Equals(interfaceType.Name) )
                 {
                     isInit = true; 
                     object classAttribute = type.GetCustomAttribute(typeof(RootProgress), false);
                     var rootProgress = classAttribute as RootProgress;
                     if (rootProgress != null)
                     { 
                         _curProgressType = type;
                     } 
                     
                     classAttribute = type.GetCustomAttribute(typeof(ProgressLoopCheck), false);
                     var progressLoopCheck = classAttribute as ProgressLoopCheck;
                     if (progressLoopCheck != null)
                     {
                         needCheck = true;
                     } 
                     break;
                 } 
             }

             if (isInit)
             {
                 var progress = Activator.CreateInstance(type) as IProgress;
                 _mProgresses.Add(type);
                 _mProgressDict.Add(type, progress); 
                 if (needCheck)
                 {
                     _needCheckProgress.Add(progress);
                 }
             } 
         }
         
         //监听对应事件
         this.Subscribe<Type>(ProgressEvent.ProgressChange,ChangeProgress);
         this.Subscribe<Type>(ProgressEvent.ProgressMakeDirty,ProgressMakeDirty);
         this.Subscribe<Type>(ProgressEvent.ProgressFinishDirt,ProgressFinishMake);
    }

    public void Update()
    {
        foreach (IProgress progress in _needCheckProgress)
        {
            progress.Check();
        }
    }

    public void ProgressFinishMake(Type progressType)
    {
        if (progressType != null)
        {
            _mFinishDict[progressType] = true;
        }
    }

    public void ProgressMakeDirty(Type progressType)
    {
        if (progressType != null)
        {
            if (_mFinishDict.ContainsKey(progressType))
            {
                _mFinishDict[progressType] = false;
            } 
        }
    }
    
    public void ChangeProgress(Type progressType)
    {
        if (progressType != null)
        {
            if (_mProgressDict.TryGetValue(progressType, out IProgress progress))
            {
                progress.Run();
                _curProgressType = progressType;
            } 
        }
    }

    public void Lunch()
    {
        DLogger.Log("Game Start!!!!");
        if (_curProgressType != null)
        {
            if (_mProgressDict.TryGetValue(_curProgressType, out IProgress progress))
            {
                progress.Run();
            } 
        }
    }
}
}
