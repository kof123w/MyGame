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
        //初始化游戏世界
        GameWorld.Instance.Init();   
        
        //todo 网络链接
        //NetManager.Instance.ConnectSvr();
         
        InputMgr.Instance.Init();
    }

    public static void Update()
    {
        //执行游戏对象的更新函数
        GameWorld.Instance.Update();
        
        InputMgr.Instance.RevInput();
    }
}