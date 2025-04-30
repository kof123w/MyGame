using System;
using System.IO;
using System.Linq;
using System.Reflection; 
using UnityEngine;  

namespace Script
{
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
        OffLine, //离线
        OnLine, //在线
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

        private Assembly hotUpdateAss;
        void Start()
        {  
            // Editor环境下，HotUpdate.dll.bytes已经被自动加载，不需要加载，重复加载反而会出问题。
#if !UNITY_EDITOR
            hotUpdateAss = Assembly.Load(File.ReadAllBytes($"{Application.streamingAssetsPath}/HotUpdate.dll.bytes")); 
#else 
            //进入游戏 
            // Editor下无需加载，直接查找获得HotUpdate程序集
           //hotUpdateAss = AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "GameScript_Runtime"); 
           hotUpdateAss = Assembly.Load(File.ReadAllBytes($"{Application.streamingAssetsPath}/HotUpdate/GameScript_Runtime.dll.bytes")); 
#endif
            Type type = hotUpdateAss.GetType("MyGame.GameMain");
            object[] parameters = { (int)logMode, (int)netMode,(int) resourceType}; // 按顺序传入参数
            type.GetMethod("LaunchGame")?.Invoke(null, parameters);
        }

        private void Update()
        {
#if UNITY_EDITOR
            // GameMain.Update();
#endif
            Type type = hotUpdateAss.GetType("MyGame.GameMain"); 
            type.GetMethod("Update")?.Invoke(null, null);

        }

        void FixedUpdate()
        {
#if UNITY_EDITOR
            // GameMain.FixedUpdate();
#endif
            Type type = hotUpdateAss.GetType("MyGame.GameMain"); 
            type.GetMethod("FixedUpdate")?.Invoke(null, null);
            
        }

        void LateUpdate()
        {
#if UNITY_EDITOR
            //GameMain.LateUpdate();
#endif
            Type type = hotUpdateAss.GetType("MyGame.GameMain"); 
            type.GetMethod("LateUpdate")?.Invoke(null, null);
        }

        void OnApplicationQuit()
        {
#if UNITY_EDITOR
            // GameMain.OnApplicationQuit();
#endif
            Type type = hotUpdateAss.GetType("MyGame.GameMain"); 
            type.GetMethod("OnApplicationQuit")?.Invoke(null, null);
        }
    }
}