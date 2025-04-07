using System;
using System.Reflection;
using Config;
using UnityEngine;

namespace MyGame
{
    public class GameMain
    {
        public static void LaunchGame(int logMode, int netMode, int resourceType)
        {
            try
            {
                #region 参数设置和前置设置
                DLogger.LogType = (DebugMode)logMode;
                ResourcerDecorator.Instance.SetLoaderResourceType(resourceType);
                NetManager.Instance.SetConnectType(netMode);
                PreProcess();
                //预加载一下配置
                ConfigPreRead.PreRead();
                #endregion
                #region 前置系统初始化   
                EventListenManager.Instance.Init();
                GameTimerManager.Instance.Init();
                ResourcerDecorator.Instance.Init();
                UIManager.Instance.Init();
                TaskManager.Instance.Init();
                PlayerInputSystem.Instance.Init(); 
                GameWorld.Instance.Init();
                #endregion
                
                //处理一下程序集合
                AssemblyProcess();  

                //最后启动流程 
                GameEvent.Push(TaskEvent.TaskLunch);
            }
            catch (Exception e)
            {
                DLogger.Error(e);
                throw;
            }
        }

        public static void Update()
        {
            try
            {
                GameTimerManager.Instance.Tick();
                UIManager.Instance.Tick();
                PlayerInputSystem.Instance.Tick();
                GameWorld.Instance.Tick();
            }
            catch(Exception e)
            {
                DLogger.Error(e);
                throw;
            }
        }

        public static void FixedUpdate()
        {
            GameWorld.Instance.FixedTick();
        }

        public static void LateUpdate()
        { 
            GameWorld.Instance.LateTick();
        }
        
        //前置处理
        private static void PreProcess()
        {
            //MainCamera DirectionalLight UIFramework StartGame GlobalVolume 这些进入不销毁到状态
            UnityEngine.Object.DontDestroyOnLoad(GameObject.Find("MainCamera"));
            UnityEngine.Object.DontDestroyOnLoad(GameObject.Find("DirectionalLight"));
            UnityEngine.Object.DontDestroyOnLoad(GameObject.Find("UIFramework"));
            UnityEngine.Object.DontDestroyOnLoad(GameObject.Find("StartGame"));
            UnityEngine.Object.DontDestroyOnLoad(GameObject.Find("GlobalVolume"));  
        }

        //这里统一进行程序集里面的程序进行初步初始化和分类避免后面其他模块多次进行程序扫描浪费性能
        private static void AssemblyProcess()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();
            var progressInterface = typeof(ITask); 
            for (int i = 0; i < types.Length; i++)
            {
                var type = types[i];
                var interfaces = type.GetInterfaces(); 
                for (int j = 0; j < interfaces.Length; j++)
                {
                    if (interfaces[j].Name.Equals(progressInterface.Name))
                    {
                        ProgressAssembly(type);
                        break;
                    } 
                } 
                var baseType = type.BaseType;
                if (baseType == typeof(BaseSubScene))
                {
                    SceneAssembly(type);
                }

                if (baseType == typeof(UIController))
                {
                    UIModelControllerAssembly(type);
                }
            }
        }

        private static void ProgressAssembly(Type type)
        { 
            System.Object classAttribute = type.GetCustomAttribute(typeof(RootProgress), false);
            if (classAttribute is RootProgress)
            {
                GameEvent.Push(TaskEvent.TaskSetCurProgress, type);
            }
            
            var progress = Activator.CreateInstance(type) as ITask;
            GameEvent.Push(TaskEvent.TaskAddProgress, type, progress);

            classAttribute = type.GetCustomAttribute(typeof(ProgressLoopCheck), false);
            if (classAttribute is ProgressLoopCheck { MIsLoopCheck: true })
            {
                //GameEvent.Push(ProgressEvent.ProgressAddNeeCheckProgress, progress);
                GameEvent.Push(TaskEvent.TaskAddNeeCheckProgress, type);
            }
        }

        private static void UIModelControllerAssembly(Type type)
        {
            if (Activator.CreateInstance(type) is UIController uiController)
            { 
                System.Object classAttribute = type.GetCustomAttribute(typeof(ControllerOf), false);
                UIModel paramModel = null;
                if (classAttribute is ControllerOf controllerOf)
                {
                    var modelType = controllerOf.ModelType; 
                    if (Activator.CreateInstance(modelType) is UIModel uiModel)
                    { 
                        paramModel = uiModel;
                        UIManager.Instance.AddModel(uiModel);
                    }
                } 
                UIManager.Instance.AddController(uiController, paramModel);
            }
        }

        private static void SceneAssembly(Type type)
        {
            System.Object classAttribute = type.GetCustomAttribute(typeof(SceneAttribute), false);
            if (classAttribute is SceneAttribute sceneAttribute)
            {
                var st = sceneAttribute.SceneType;
                var sceneId = sceneAttribute.SceneId;
                if (Activator.CreateInstance(type) is BaseSubScene scene)
                {
                    scene.SetScene(sceneId, st); 
                    GameWorld.Instance.AddSubScene(scene);
                }
            }
        }
    }
}