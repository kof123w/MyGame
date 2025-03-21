using Config;
using MyGame;

public class GameMain
{
    public static void LaunchGame(int logMode)
    {
        DLogger.LogType = (DebugMode)logMode; 
        //初始化事件系统
        EventListenMgr.Instance.Init();
        //计时器环境初始化
        TimerManger.Instance.Init();
        //游戏物理世界创建
        PhysicsSystem.Instance.Initialize();
        //初始化游戏世界
        GameWorld.Instance.Init();   
        
        //todo 网络链接
        //NetManager.Instance.ConnectSvr();
        //预加载一下配置
        ConfigPreRead.PreRead();
        InputMgr.Instance.Init(); 
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public static void Update()
    {
        //执行游戏对象的更新函数
        GameWorld.Instance.Update();
        InputMgr.Instance.RevInput();
    }
}