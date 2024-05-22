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
        containDll.Add("Framework.Hotfix.dll");
        containDll.Add("Framework.Hotfix.pdb");
        containDll.Add("Framework.Core.dll");
        containDll.Add("Framework.Core.pdb");
        string path = "Temp\\Bin\\Debug\\Hotfix";
        string destinPath = "Assets\\StreamingAssets\\AotDll";

        if (Directory.Exists(destinPath))
        {
            string[] files = Directory.GetFiles(destinPath);
            foreach (string file in files)
            {
                if (file.Contains(".meta"))
                {
                    continue;
                }

                File.Delete(file);
            }
        }

        if (Directory.Exists(path))
        {
            string[] files = Directory.GetFiles(path);
            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file);
                string destin = Path.Combine(destinPath);
                if (containDll.Contains(fileName))
                { 
                    File.Copy(file,string.Format("{0}\\{1}",destinPath,fileName));
                    Debug.Log("移动文件"+fileName);
                }
            }
        }
    }
}
