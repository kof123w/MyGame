using UnityEngine;
#if UNITY_LOCAL_SCRIPT
using MyGame;
#endif

enum LogMode
{
    None,
    AllLog,
    Warring,
    WarringOrError,
    Error
}

enum NetMode
{
    OffLine,  //离线
    OnLine,  //在线
}

enum ResourceLoadType
{
    Local,
    Cdn,
} 

class Launch : MonoBehaviour
{
    [Header("日志模式")] public LogMode logMode;
    [Header("网络模式")] public NetMode netMode;
    [Header("资源模式")] public ResourceLoadType resourceType;
    void Start()
    { 
#if UNITY_LOCAL_SCRIPT
        //进入游戏 
        GameMain.LaunchGame((int)logMode,(int)netMode,(int)resourceType); 
#endif
    }

    private void Update()
    {
#if UNITY_LOCAL_SCRIPT
        //进入游戏 
        GameMain.Update(); 
#endif
        
    }

    void LateUpdate()
    {
        #if UNITY_LOCAL_SCRIPT
        GameMain.LateUpdate();
        #endif
    }
}