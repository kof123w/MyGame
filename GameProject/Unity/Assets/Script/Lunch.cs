using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Cysharp.Threading.Tasks;
using HybridCLR;
#if UNITY_LOCAL_SCRIPT
using MyGame;
#endif
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

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

    class Lunch : MonoBehaviour
    {
        [Header("日志模式")] public LogMode logMode;
        [Header("网络模式")] public NetMode netMode;
        [Header("资源模式")] public ResourceLoadType resourceType;

        private GameVersion gameVersion;
#if !UNITY_LOCAL_SCRIPT
        private Assembly hotUpdateAss;
#endif


#if !UNITY_LOCAL_SCRIPT
        private async void Start()
        { 
            await LoadMetaDll();
            //加载下来所有的dll.bytes
            //List<string> keys = new List<string>() { "HotUpdateDll" };
            List<string> hotDllName = new List<string>() { "GameScript_Core","GameScript_Proto","GameScript_Config","GameScript_Frame","GameScript_Runtime" };
            //var handlers = Addressables.LoadAssetsAsync<TextAsset>(keys, null, Addressables.MergeMode.Union);
            //await handlers;
            //一词加载依赖和运行dll
            foreach (string dll in hotDllName)
            {
                Debug.Log(dll);
                //加载程序集
                var handler = await Addressables.LoadAssetAsync<TextAsset>(dll); 
                var assembly = Assembly.Load(handler.bytes);
                if (dll.Equals("GameScript_Runtime"))
                {
                    hotUpdateAss = assembly;
                    Type type = hotUpdateAss.GetType("MyGame.GameMain");
                    object[] parameters = { (int)logMode, (int)netMode, (int)resourceType }; // 按顺序传入参数
                    type.GetMethod("LaunchGame")?.Invoke(null, parameters); 
                }
                Addressables.Release(handler);
            }   
#else
        private void Start()
        {  
            Debug.LogError("lunch game...Start");
            //进入游戏   
            //hotUpdateAss = Assembly.Load(File.ReadAllBytes($"{Application.streamingAssetsPath}/HotUpdate/GameScript_Runtime.dll.bytes"));  
            GameMain.LaunchGame((int)logMode, (int)netMode, (int)resourceType);
#endif
        }

        private void Update()
        {
#if !UNITY_LOCAL_SCRIPT
            if (hotUpdateAss != null)
            {
                Type type = hotUpdateAss.GetType("MyGame.GameMain");
                type.GetMethod("Update")?.Invoke(null, null);
            }
#else
            GameMain.Update();
#endif
        }

        private void FixedUpdate()
        {
#if UNITY_LOCAL_SCRIPT
            GameMain.FixedUpdate();
#else
            if (hotUpdateAss != null)
            {
                Type type = hotUpdateAss.GetType("MyGame.GameMain");
                type.GetMethod("FixedUpdate")?.Invoke(null, null);
            }
#endif
        }

        private void LateUpdate()
        {
#if UNITY_LOCAL_SCRIPT
            GameMain.LateUpdate();
#else
            if (hotUpdateAss != null)
            {
                Type type = hotUpdateAss.GetType("MyGame.GameMain");
                type.GetMethod("LateUpdate")?.Invoke(null, null);
            }
#endif
        }

        private void OnApplicationQuit()
        {
#if UNITY_LOCAL_SCRIPT
            GameMain.OnApplicationQuit();
#else
            if (hotUpdateAss != null)
            {
                Type type = hotUpdateAss.GetType("MyGame.GameMain");
                type.GetMethod("OnApplicationQuit")?.Invoke(null, null);
            }
#endif
        }

#if !UNITY_LOCAL_SCRIPT
        
        private static async UniTask LoadMetaDll()
        {
            List<string> keys = new List<string>() { "AOTDll" };
            var handler = Addressables.LoadAssetsAsync<TextAsset>(keys, null, Addressables.MergeMode.Union);
            handler.Completed += operation =>
            {
                if (operation.Status != AsyncOperationStatus.Succeeded)
                {
                    Debug.LogError($"加载失败: {operation.OperationException}");
                    Debug.LogException(operation.OperationException); // 这将打印完整堆栈
                }
            };
            await handler; 

            foreach (var result in handler.Result)
            {
                LoadImageErrorCode err =
                    RuntimeApi.LoadMetadataForAOTAssembly(result.bytes, HomologousImageMode.SuperSet);
#if DEBUG_LOG
                Debug.LogWarning($"LoadMetadataForAOTAssembly:{result.name} ret:{err}");
#endif 
            }
            
            Addressables.Release(handler);
        }
#endif
    }
}