using System;
using UnityEditor;
using UnityEngine;

namespace MyGame.Editor
{
    public class ExcelWindow : EditorWindow
    {

        private string configPath = string.Empty;
        private string outputKey = EditorConst.excelOutputKey;   //导入到config路径
        private string inputKey = EditorConst.excelInputKey;  //编辑器导出的config路径
        [MenuItem("Excel/ExcelWindow")]
        private static void ShowExcelProcessWindow()
        { 
            var window = (ExcelWindow)GetWindow(typeof(ExcelWindow), false, "配置表", false);
            window.autoRepaintOnSceneChange = true;
            window.Init();
            window.Show(true);  
        }

        public void Init()
        {
            string str = EditorPrefs.GetString(outputKey);
            configPath = string.IsNullOrEmpty(str) ? "" : str;
        }

        private void OnGUI()
        {
            GUILayout.Label("配置路径"); 
            GUILayout.Space(5);
            string oldPath = configPath;
            configPath = GUILayout.TextField(configPath);
            if (!configPath.Equals(oldPath))
            {
                EditorPrefs.SetString(outputKey,configPath);
            }
            GUILayout.Space(10);
            if (GUILayout.Button("导入配置"))
            {
                Debug.Log("导入配置表，路径:"+configPath);
                ExcelTool.RebuildConfig(configPath); 
                AssetDatabase.ImportAsset("Assets\\Script\\GameScript\\Excel\\ConfigVO", ImportAssetOptions.ForceUpdate);
                AssetDatabase.ImportAsset("Assets\\StreamingAssets\\Config", ImportAssetOptions.ForceUpdate);
            } 

        } 
    }
}
