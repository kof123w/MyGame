using UnityEditor;
using UnityEngine;

namespace MyGame.Editor
{
    public class ProtoWindow : EditorWindow
    {

        private string configPath = string.Empty;
        private string outputKey = EditorConst.excelOutputKey;   //导入到config路径 
        
        private string protocPath = string.Empty;
        private string protocOutputKey = EditorConst.protocPath;
        
        [MenuItem("Proto/ProtoWindow")]
        private static void ShowExcelProcessWindow()
        { 
            var window = (ProtoWindow)GetWindow(typeof(ProtoWindow), false, "配置和协议生成", false);
            window.autoRepaintOnSceneChange = true;
            window.Init();
            window.Show(true);  
        }

        public void Init()
        {
            string str = EditorPrefs.GetString(outputKey);
            configPath = string.IsNullOrEmpty(str) ? "" : str;
            
            string protoStr = EditorPrefs.GetString(protocOutputKey);
            protocPath = string.IsNullOrEmpty(protoStr) ? "" : protoStr;
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
            
            GUILayout.Label("Protoc.exe路径"); 
            GUILayout.Space(5);
            string oldProtocPath = protocPath;
            protocPath = GUILayout.TextField(protocPath);
            if (!protocPath.Equals(oldProtocPath))
            {
                EditorPrefs.SetString(protocOutputKey, protocPath);
            }

            GUILayout.Space(10);
            if (GUILayout.Button("点我生成配置Proto VO类"))
            {
                Debug.Log("导入配置表，路径:"+configPath);
                ProtoExcelGenTool.ReBuildConfigVo(configPath,protocPath);  
                AssetDatabase.ImportAsset("Assets/Script/GameScript/Excel/ConfigVO", ImportAssetOptions.ImportRecursive);
            } 
            
            if (GUILayout.Button("生成Proto VO类，重新生成DLL，点我生成配置文件"))
            {
                ProtoExcelGenTool.ReBuildConfig(configPath);
                AssetDatabase.ImportAsset("Assets/Resources/Config", ImportAssetOptions.ImportRecursive);
            } 
        } 
    }
}
