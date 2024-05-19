using System.Collections.Generic;
using System.IO;
using System.Security.Policy;
using UnityEngine;
using UnityEditor;

public static class CopyDll
{  
    [MenuItem("AotDll/CopyDllToAotDll")]
    public static void MergeSprite()
    {
        HashSet<string> containDll = new HashSet<string>();
        containDll.Add("Library/ScriptAssemblies\\Hotfix.dll");
        containDll.Add("Library/ScriptAssemblies\\Hotfix.pdb");
        containDll.Add("Library/ScriptAssemblies\\Core.dll");
        containDll.Add("Library/ScriptAssemblies\\Core.pdb");
        string path = "Library/ScriptAssemblies";
        string destinPath = "Assets\\AotDll";
        if (Directory.Exists(path))
        {
            string[] files = Directory.GetFiles(path);
            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file);
                string destin = Path.Combine(destinPath);
                if (containDll.Contains(fileName))
                {
                    File.Move(file,destin);
                }
            }
        }
    }
}
