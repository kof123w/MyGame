using System;
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
                #region 参数设置 
                DLogger.LogType = (DebugMode)logMode;
                ResourceLoader.Instance.SetLoaderResourceType(resourceType);
                NetManager.Instance.SetConnectType(netMode);

                #endregion

                #region 前置系统启动

                //初始化事件系统
                EventListenManager.Instance.Init();
                //计时器环境初始化
                TimerManger.Instance.Init();
                //游戏物理世界创建
                //PhysicsSystem.Instance.Initialize(); 
                //UI系统
                UIManager.Instance.InitUIManager();

                #endregion

                #region 资源相关启动

                //预加载一下配置
                ConfigPreRead.PreRead();

                #endregion

                #region 流程相关

                ProgressManager.Instance.Init();

                #endregion

                //初始化游戏世界
                GameWorld.Instance.Init();
                InputMgr.Instance.Init();

                //最后启动流程
                ProgressManager.Instance.Lunch();
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
    }
}