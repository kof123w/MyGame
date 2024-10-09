using UnityEngine;

enum LogMode
{
    None,
    AllLog,
    Warring,
    WarringOrError,
    Error
}

class Launch : MonoBehaviour
{
    [Header("日志模式")] public LogMode logMode;

    void Start()
    {
        //初始化框架参数
        // todo

        //资源管理..版本更新
        // todo 
        
        //进入游戏 
#if UNITY_EDITOR
        GameMain.LaunchGame((int)logMode);
#else
   HotfixManager.Instance.InitAssembly((int)logMode);
#endif
    }

    private void Update()
    {
#if UNITY_EDITOR
        GameMain.Update();
#else
        HotfixManager.Instance.Update();
#endif
    }
}