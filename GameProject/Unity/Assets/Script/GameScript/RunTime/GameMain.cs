using System;
using System.Reflection;
using AssetsLoad;
using Config;
using DebugTool;
using EventSystem;
using GameTimer;
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
                NetManager.Instance.Init();
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
            try
            {
                GameWorld.Instance.FixedTick(); 
                PlayerInputSystem.Instance.RevInput();
                FrameLogic.Instance.FixedTick();
            }
            catch(Exception e)
            {
                DLogger.Error(e);
                throw;
            }
            
        }

        public static void LateUpdate()
        { 
            try
            {
                GameWorld.Instance.LateTick();
            }
            catch(Exception e)
            {
                DLogger.Error(e);
                throw;
            }
           
        }

        //监听程序退出
        public static void OnApplicationQuit()
        {
            UDPNetManager.Instance.Disconnect();
            NetManager.Instance.Disconnect();
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
            var taskInterface = typeof(ITask);
            var netInterface = typeof(INetHandler);
            foreach (var type in types)
            {
                var interfaces = type.GetInterfaces(); 
                foreach (var t in interfaces)
                {
                    if (t.Name.Equals(taskInterface.Name))
                    {
                        ProgressAssembly(type);
                        break;
                    } 
                    
                    if (t.Name.Equals(netInterface.Name))
                    {
                        NetHandleAssembly(type);
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

                if (baseType == typeof(DataClass))
                {
                    DataClassAssembly(type);
                }
            }
        } 
     
        private static void ProgressAssembly(Type type)
        { 
            System.Object classAttribute = type.GetCustomAttribute(typeof(RootTask), false);
            if (classAttribute is RootTask)
            {
                GameEvent.Push(TaskEvent.TaskSetCurTask, type);
            }
            
            var progress = Activator.CreateInstance(type) as ITask;
            GameEvent.Push(TaskEvent.TaskAddTask, type, progress); 
        }

        private static void UIModelControllerAssembly(Type type)
        {
            if (Activator.CreateInstance(type) is UIController uiController)
            { 
                UIManager.Instance.AddController(uiController);
            }
        }
        
        private static void NetHandleAssembly(Type type)
        {
            if (Activator.CreateInstance(type) is INetHandler netHandler)
            { 
                System.Object udpHandler = type.GetCustomAttribute(typeof(UDPHandler), false);
                UDPHandler udpHandlerInstance = (UDPHandler)udpHandler;
                if (udpHandlerInstance!=null && udpHandlerInstance.UseUDP )
                { 
                    UDPNetManager.Instance.AddHandler(netHandler);
                }
                else
                {
                    NetManager.Instance.AddHandler(netHandler);
                } 
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

        private static void DataClassAssembly(Type type)
        {
            if (Activator.CreateInstance(type) is DataClass dataClass)
            { 
                DataCenterManger.Instance.AddDataClass(type,dataClass);
            }
        }
    }
}