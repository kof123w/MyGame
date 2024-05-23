using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class ProcessExcel
{
    [MenuItem("ProcessExcel/ReBuildConfig")]
    public static void RebuildConfig()
    {
        string generaPath = "StreamingAssets\\Config";
        string excelPath = "";
        if (!Directory.Exists(generaPath))
        {
            Directory.CreateDirectory(generaPath);
        }
    }
}
