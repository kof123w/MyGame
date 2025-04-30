using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class AotDllCopyTool
{
    private const string Path = "Assets/../HybridCLRData/HotUpdateDlls/StandaloneWindows64";
    private static readonly string CopyPath = $"{Application.streamingAssetsPath}/HotUpdate";
    private static readonly string AllDllCopyPath = $"Assets/AotDll";
    [MenuItem("Assets/CustomTool/AotDllCopy")]
    public static void DoAotDllCopy()
    {  
        List<string> dllNames = new List<string>()
        {
            "GameScript_Config",
            "GameScript_Core",
            "GameScript_Frame",
            "GameScript_Proto",
            "GameScript_Runtime",
        };
        if (!Directory.Exists(CopyPath))
        {
            Directory.CreateDirectory(CopyPath);
        }
        
        string[] files = Directory.GetFiles(Path);

        foreach (var file in files)
        {
            if (IsContainDll(file, dllNames))
            {
                string fileName = System.IO.Path.GetFileName(file);
                File.Copy(file, $"{CopyPath}/{fileName}.bytes", true);
            }
        } 
    }
    
    [MenuItem("Assets/CustomTool/AllDllCopy")]
    public static void DoAllDllCopy()
    { 
        if (!Directory.Exists(AllDllCopyPath))
        {
            Directory.CreateDirectory(AllDllCopyPath);
        }
        
        string[] files = Directory.GetFiles(Path);

        foreach (var file in files)
        {
            string fileName = System.IO.Path.GetFileName(file);
            File.Copy(file, $"{AllDllCopyPath}/{fileName}", true);
            AssetDatabase.ImportAsset("Assets/Resources/Config", ImportAssetOptions.ImportRecursive);
        } 
    }

    public static bool IsContainDll(string dllName,List<string> dllNames)
    {
        bool result = false;
        for (int i = 0; i < dllNames.Count; i++) {
            if (dllName.Contains(dllNames[i]))
            {
                result = true;
                break;
            }
        }
        
        return result;
    }
}
