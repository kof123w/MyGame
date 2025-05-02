using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class AotDllCopyTool
{
    
    private static readonly string AOTDllPath = $"Assets/../HybridCLRData/HotUpdateDlls/{EditorUserBuildSettings.activeBuildTarget}";
    private static readonly string AotDllCopyPath = $"Assets/Resources_moved/HotUpdate";
    
    private static readonly string MetaDllPath = $"Assets/../HybridCLRData/AssembliesPostIl2CppStrip/{EditorUserBuildSettings.activeBuildTarget}";
    private static readonly string MetaDllCopyPath = $"Assets/Resources_moved/MetaDll";
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
        if (!Directory.Exists(AotDllCopyPath))
        {
            Directory.CreateDirectory(AotDllCopyPath);
        }
        
        string[] files = Directory.GetFiles(AOTDllPath);

        foreach (var file in files)
        {
            if (IsContainDll(file, dllNames))
            { 
                string fileName = System.IO.Path.GetFileName(file);
                if (!fileName.EndsWith(".pdb"))
                {
                    fileName = fileName.Replace(".dll", ".bytes");
                    File.Copy(file, $"{AotDllCopyPath}/{fileName}", true); 
                } 
            }
        } 
        
        AssetDatabase.ImportAsset($"{AotDllCopyPath}", ImportAssetOptions.ImportRecursive);
        
        List<string> metaDllNames = new List<string>()
        {
            "Google.Protobuf.dll",
            "System.Core.dll",
            "System.dll",
            "UniTask.Addressables.dll",
            "UniTask.dll",
            "Unity.Addressables.dll",
            "Unity.ResourceManager.dll",
            "UnityEngine.CoreModule.dll",
            "mscorlib.dll",
        };
        
        //元数据补充dll拷贝
        if (!Directory.Exists(MetaDllCopyPath))
        {
            Directory.CreateDirectory(MetaDllCopyPath);
        }
        
        string[] metaFiles = Directory.GetFiles(MetaDllPath);

        foreach (var file in metaFiles)
        {
            if (IsContainDll(file, metaDllNames))
            {
                string fileName = System.IO.Path.GetFileName(file);
                fileName = fileName.Replace(".dll", ".bytes");
                File.Copy(file, $"{MetaDllCopyPath}/{fileName}", true); 
            }
        } 
        AssetDatabase.ImportAsset($"{MetaDllCopyPath}", ImportAssetOptions.ImportRecursive);
    }
    
    /*[MenuItem("Assets/CustomTool/AllDllCopy")]
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
    }*/

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
