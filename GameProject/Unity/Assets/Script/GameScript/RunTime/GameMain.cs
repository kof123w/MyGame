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
                ResourceLoader.Instance.SetLoaderResourceType(resourceType);
                NetManager.Instance.SetConnectType(netMode);
                PreProcess();
                //预加载一下配置
                ConfigPreRead.PreRead();
                #endregion

                #region 前置系统初始化 
                //初始化事件系统
                EventListenManager.Instance.Init();
                //计时器环境初始化
                TimerManger.Instance.Init(); 
                //UI系统
                UIManager.Instance.InitUIManager(); 
                //流程
                ProgressManager.Instance.Init();
                #endregion  

                //处理一下程序集合
                AssemblyProcess();
                
                //初始化游戏世界
                GameWorld.Instance.Init();
                InputMgr.Instance.Init();

                //最后启动流程 
                GameEvent.Push(ProgressEvent.ProgressLunch);
            }
            catch (Exception e)
            {
                DLogger.Error(e);
                throw;
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public static void Update()
        {
            try
            {
                //执行游戏对象的更新函数
                GameWorld.Instance.Update();
                InputMgr.Instance.RevInput();
                UIManager.Instance.Update();
                ProgressManager.Instance.Update();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        public static void LateUpdate()
        {
            //todo ..
            GameWorld.Instance.LateUpdate();
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
            var progressInterface = typeof(IProgress);
            var uiControllerInterface = typeof(IUIController);
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

                    if (interfaces[j].Name.Equals(uiControllerInterface.Name))
                    {
                        UIControllerAssembly(type);
                        break;
                    }
                } 
            }
        }

        private static void ProgressAssembly(Type type)
        { 
            System.Object classAttribute = type.GetCustomAttribute(typeof(RootProgress), false);
            if (classAttribute is RootProgress)
            {
                GameEvent.Push(ProgressEvent.ProgressSetCurProgress, type);
            }
            
            var progress = Activator.CreateInstance(type) as IProgress;
            GameEvent.Push(ProgressEvent.ProgressAddProgress, type, progress);

            classAttribute = type.GetCustomAttribute(typeof(ProgressLoopCheck), false);
            if (classAttribute is ProgressLoopCheck { MIsLoopCheck: true })
            {
                GameEvent.Push(ProgressEvent.ProgressAddNeeCheckProgress, progress);
            }
        }

        private static void UIControllerAssembly(Type type)
        {
            if (Activator.CreateInstance(type) is IUIController uiController)
            { 
                GameEvent.Push(UIEvent.UIManagerEvent_AddUIController, uiController);
            }
        }
    }
}