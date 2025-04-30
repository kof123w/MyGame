using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MyGame.Editor
{
    [CreateAssetMenu(fileName = "GameLunch", menuName = "CreateAsset")]
    public class GameLunchAsset : ScriptableObject
    {
        public int age;
        public string username;
        public string password;
    } 

    public class CreateAssetEditor
    {
        [MenuItem("Assets/CreateGameLunchAsset")]
        static void CreateScriptObject()
        {
            GameLunchAsset createAsset = ScriptableObject.CreateInstance<GameLunchAsset>();
            createAsset.age = 18;
            createAsset.username = "lisi";
            createAsset.password = "111111";

            AssetDatabase.CreateAsset(createAsset, "Assets/Resources/GameLunch.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}