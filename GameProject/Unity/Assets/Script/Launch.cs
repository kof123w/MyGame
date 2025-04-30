using UnityEngine;
#if UNITY_EDITOR
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
#if UNITY_EDITOR
        //进入游戏 
        //GameMain.LaunchGame((int)logMode,(int)netMode,(int)resourceType); 
#endif
    }

    private void Update()
    {
#if UNITY_EDITOR 
       // GameMain.Update();
#endif
        
    }

    void FixedUpdate()
    {
#if UNITY_EDITOR 
       // GameMain.FixedUpdate();
#endif
    }

    void LateUpdate()
    {
#if UNITY_EDITOR
        //GameMain.LateUpdate();
#endif
    }

    void OnApplicationQuit()
    {
#if UNITY_EDITOR
       // GameMain.OnApplicationQuit();
#endif 
    }
}