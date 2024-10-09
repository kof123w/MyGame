using System;
using UnityEditor;
using UnityEngine;

public class ExcelWindow : EditorWindow
{

   private string m_configPath = string.Empty;
   private string key = "ExcelPathGlo";
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
       string str = EditorPrefs.GetString(key);
       m_configPath = string.IsNullOrEmpty(str) ? "" : str;
   }

   private void OnGUI()
   {
       GUILayout.Label("配置路径"); 
       GUILayout.Space(5);
       string oldPath = m_configPath;
       m_configPath = GUILayout.TextField(m_configPath);
       if (!m_configPath.Equals(oldPath))
       {
           EditorPrefs.SetString(key,m_configPath);
       }
       GUILayout.Space(10);
       if (GUILayout.Button("导入配置"))
       {
           Debug.Log("导入配置表，路径:"+m_configPath);
           ExcelTool.RebuildConfig(m_configPath);
           //EditorUtility.DisplayDialog("确认", "导入配置完成!", "OK");
       } 

   }

   private void OnInspectorUpdate()
   {  
   }
}
