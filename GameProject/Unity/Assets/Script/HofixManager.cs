using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using MyGame;
public class HotfixManager : Singleton<HotfixManager>
{ 
    private Assembly m_hotUpdateAss;
    private Type m_gameMainType;
    
    public void InitAssembly(int logMode)
    {
        LoadHotfixAssembly("GameCore.dll.bytes");
        if (m_hotUpdateAss!=null)
        {
            m_gameMainType = m_hotUpdateAss.GetType("GameMain");
            if (m_gameMainType!=null)
            {
                m_gameMainType.GetMethod("LaunchGame").Invoke(null,new object[]
                {
                    logMode,
                });
            }
        }
    }

    private void LoadHotfixAssembly(string dllName)
    {
        string dllPath = Path.Combine(Application.streamingAssetsPath, "AotDll", dllName);
        Debug.Log($"Attempting to load DLL from path: {dllPath}");

        if (File.Exists(dllPath))
        {
            try
            {
                byte[] dllBytes = File.ReadAllBytes(dllPath);
                m_hotUpdateAss = Assembly.Load(dllBytes);
                Debug.Log($"Loaded {dllName} successfully.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to load assembly: {dllName}. Exception: {ex}");
            }
        }
        else
        {
            Debug.LogError($"DLL not found at path: {dllPath}");
        } 
    }

    public void Update()
    {
        m_gameMainType = m_hotUpdateAss.GetType("GameMain");
        if (m_gameMainType!=null)
        {
            m_gameMainType.GetMethod("Update").Invoke(null,null);
        }
    }
}