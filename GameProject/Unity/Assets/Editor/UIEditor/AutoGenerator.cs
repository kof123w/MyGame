using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace MyGameEditor
{
    public class AutoGenerator
    {
        private static string textName = "m_text";
        private static string imageName = "m_img";
        private static string goName = "m_go";
        private static string tfName = "m_tf";
        private static string btnName = "m_btn";
        private static string inputText = "m_input";
        private static string rectName = "m_rect";
        private static string itemName = "m_item";

        private static List<string> m_pathList = new List<string>();
        private static List<Object> m_objectList = new List<Object>();
        private static List<string> m_nameList = new List<string>();

        [MenuItem("GameObject/UI/AutoFieldGenerator")]
        private static void GenerateUIField()
        {
            Object activeObject = Selection.activeObject;
            if (activeObject == null)
            {
                Debug.LogError("No GameObject selected!");
                return;
            }
            
            StringBuilder sb = new StringBuilder(); 
            // 加载原始 Prefab（可修改并保存）
            GameObject go = (GameObject)activeObject; 
            Transform tf = go.transform;
            m_pathList.Clear();
            m_objectList.Clear();
            m_nameList.Clear();
            /*var refRoot = go.GetComponent<RefRoot>();
            if (refRoot != null)
            {
                refRoot.ClearRef();
            }
            else
            {
                refRoot = go.AddComponent<RefRoot>();
            }*/

            sb.Append("    #region 自动生成\n"); 
            WriteField(tf, sb);
                
            CalcTypeList(tf);
            sb.Append("    public override void OnUIAwake()\n");
            sb.Append("    {\n"); 
            sb.Append($"    base.OnUIAwake();\n");   
            WriteMethod( sb);  
            sb.Append("    }\n");
            sb.Append("    #endregion\n");
            GUIUtility.systemCopyBuffer = sb.ToString();
                
            // 保存修改
            //PrefabUtility.SavePrefabAsset(go);
            //PrefabUtility.UnloadPrefabContents(go);
        } 

        private static void WriteField(Transform tf, StringBuilder sb)
        {
            if (tf.childCount <= 0)
            {
                return;
            }

            for (int i = 0; i < tf.childCount; i++)
            {
                var childTrans = tf.GetChild(i);
                if (!childTrans.name.Contains(itemName))
                { 
                    WriteField(childTrans, sb);
                }
               
                if (childTrans.name.Contains(" "))
                {
                    continue;
                }

                if (childTrans.name.Contains(textName))
                {
                    sb.Append($"    public Text {childTrans.name};\n");
                }
                else if (childTrans.name.Contains(imageName))
                {
                    sb.Append($"    public Image {childTrans.name};\n");
                }
                else if (childTrans.name.Contains(goName))
                {
                    sb.Append($"    public GameObject {childTrans.name};\n");
                }
                else if (childTrans.name.Contains(tfName))
                {
                    sb.Append($"    public Transform {childTrans.name};\n");
                }
                else if (childTrans.name.Contains(btnName))
                {
                    sb.Append($"    public Button {childTrans.name};\n");
                }
                else if (childTrans.name.Contains(inputText))
                {
                    sb.Append($"    public InputField {childTrans.name};\n");
                }
                else if (childTrans.name.Contains(rectName))
                {
                    sb.Append($"    public RectTransform {childTrans.name};\n");
                }else if (childTrans.name.Contains(itemName))
                {
                    sb.Append($"    public GameObject {childTrans.name};\n");
                }
            }
        }

        private static void CalcTypeList(Transform tf)
        {
            for (int i = 0; i < tf.childCount; i++)
            {
                CalcTypeListStack(tf,"");
            }
        }

        private static void CalcTypeListStack(Transform tf,string path)
        {
            if (tf.childCount <= 0)
            {
                return;
            }

            for (int i = 0; i < tf.childCount; i++)
            {
                var childTrans = tf.GetChild(i);
                string pathTmp = $"{path}{childTrans.name}";
                if (!childTrans.name.Contains(itemName))
                { 
                    CalcTypeListStack(childTrans,$"{pathTmp}/");
                }
                
                if (childTrans.name.Contains(" "))
                {
                    continue;
                }
                
                if (childTrans.name.Contains(textName))
                {
                    m_pathList.Add(pathTmp);
                    m_objectList.Add(childTrans.GetComponent<Text>());
                    m_nameList.Add(childTrans.name);
                }
                else if (childTrans.name.Contains(imageName))
                {
                    m_pathList.Add(pathTmp);
                    m_objectList.Add(childTrans.GetComponent<Image>());
                    m_nameList.Add(childTrans.name);
                }
                else if (childTrans.name.Contains(goName))
                {
                    m_pathList.Add(pathTmp);
                    m_objectList.Add(childTrans.gameObject);
                    m_nameList.Add(childTrans.name);
                }
                else if (childTrans.name.Contains(tfName))
                {
                    m_pathList.Add(pathTmp);
                    m_objectList.Add(childTrans);
                    m_nameList.Add(childTrans.name);
                }
                else if (childTrans.name.Contains(btnName))
                {
                    m_pathList.Add(pathTmp);
                    m_objectList.Add(childTrans.GetComponent<Button>());
                    m_nameList.Add(childTrans.name);
                }
                else if (childTrans.name.Contains(inputText))
                {
                    m_pathList.Add(pathTmp);
                    m_objectList.Add(childTrans.GetComponent<InputField>());
                    m_nameList.Add(childTrans.name);
                }
                else if (childTrans.name.Contains(rectName))
                {
                    m_pathList.Add(pathTmp);
                    m_objectList.Add(childTrans.GetComponent<RectTransform>());
                    m_nameList.Add(childTrans.name);
                    //sb.Append($"        {childTrans.name} = m_obj.transform.Find(\"{tmp}/{childTrans.name}\").GetComponent<RectTransform>();\n"); 
                }else if (childTrans.name.Contains(itemName))
                {
                    m_pathList.Add(pathTmp);
                    m_objectList.Add(childTrans.gameObject);
                    m_nameList.Add(childTrans.name);
                }
            }
        }

        private static void WriteMethod(StringBuilder sb)
        {
            for (int i = 0; i < m_pathList.Count; i++)
            {
                if (m_nameList[i].Contains(" "))
                {
                    continue;
                } 
                if (m_nameList[i].Contains(textName))
                {
                    sb.Append($"          {m_nameList[i]} = GameObject.Find(\"{m_pathList[i]}\").GetComponent<Text>();\n"); 
                }
                else if (m_nameList[i].Contains(imageName))
                {
                    sb.Append($"          {m_nameList[i]} = GameObject.Find(\"{m_pathList[i]}\").GetComponent<Image>();\n"); 
                }
                else if (m_nameList[i].Contains(goName))
                {
                    sb.Append($"          {m_nameList[i]}  = GameObject.Find(\"{m_pathList[i]}\").gameObject;\n"); 
                }
                else if (m_nameList[i].Contains(tfName))
                {
                    sb.Append($"         {m_nameList[i]} = GameObject.Find(\"{m_pathList[i]}\").transform;\n"); 
                }
                else if (m_nameList[i].Contains(btnName))
                {
                    sb.Append($"          {m_nameList[i]} = GameObject.Find(\"{m_pathList[i]}\").GetComponent<Button>();\n"); 
                }
                else if (m_nameList[i].Contains(inputText))
                {
                    sb.Append($"          {m_nameList[i]} = GameObject.Find(\"{m_pathList[i]}\").GetComponent<InputField>();\n"); 
                }
                else if (m_nameList[i].Contains(rectName))
                {
                    sb.Append($"          {m_nameList[i]} = GameObject.Find(\"{m_pathList[i]}\").GetComponent<RectTransform>();\n"); 
                }else if (m_nameList[i].Contains(itemName))
                {
                    sb.Append($"          {m_nameList[i]} = GameObject.Find(\"{m_pathList[i]}\").gameObject;\n"); 
                }
            }
        }
    }
}