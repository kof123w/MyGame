using UnityEngine;

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

class Launch : MonoBehaviour
{
    [Header("日志模式")] public LogMode logMode;
    [Header("网络模式")] public NetMode netMode;

    void Start()
    { 
        //进入游戏 
#if UNITY_EDITOR
        GameMain.LaunchGame((int)logMode); 
#endif
    }

    private void Update()
    {
#if UNITY_EDITOR
        GameMain.Update(); 
#endif
    }
}