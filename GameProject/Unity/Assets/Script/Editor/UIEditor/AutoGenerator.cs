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

        private static List<string> m_indexType = new List<string>();
        private static List<Object> m_objectList = new List<Object>();
        private static List<string> m_nameList = new List<string>();

        [MenuItem("GameObject/UI/AutoFieldGenerator")]
        private static void GenerateUIField()
        {
            Object activeObject = Selection.activeObject;
            if (activeObject is GameObject)
            {
                StringBuilder sb = new StringBuilder();
                GameObject go = activeObject as GameObject;
                Transform tf = go.transform;
                m_indexType.Clear();
                m_objectList.Clear();
                m_nameList.Clear();
                var refRoot = go.GetComponent<RefRoot>();
                if (refRoot != null)
                {
                    refRoot.ClearRef();
                }
                else
                {
                    refRoot = go.AddComponent<RefRoot>();
                }

                sb.Append("    #region 自动生成\n"); 
                WriteField(tf, sb);
                
                CalcTypeList(tf);
                sb.Append("    public override void OnAwake()\n");
                sb.Append("    {\n"); 
                sb.Append($"    base.OnAwake();\n");  
                sb.Append("     RefRoot refRoot = MGameObject.GetComponent<RefRoot>();\n");
                WriteMethod(go,refRoot, sb);  
                sb.Append("    }\n");
                sb.Append("    #endregion\n");
                GUIUtility.systemCopyBuffer = sb.ToString();
                
                // 保存修改
                PrefabUtility.SavePrefabAsset(go);
            }
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
                CalcTypeListStack(tf);
            }
        }

        private static void CalcTypeListStack(Transform tf)
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
                    CalcTypeListStack(childTrans);
                }
                
                if (childTrans.name.Contains(" "))
                {
                    continue;
                }

                if (childTrans.name.Contains(textName))
                {
                    m_indexType.Add(textName);
                    m_objectList.Add(childTrans.GetComponent<Text>());
                    m_nameList.Add(childTrans.name);
                }
                else if (childTrans.name.Contains(imageName))
                {
                    m_indexType.Add(imageName);
                    m_objectList.Add(childTrans.GetComponent<Image>());
                    m_nameList.Add(childTrans.name);
                }
                else if (childTrans.name.Contains(goName))
                {
                    m_indexType.Add(goName);
                    m_objectList.Add(childTrans.gameObject);
                    m_nameList.Add(childTrans.name);
                }
                else if (childTrans.name.Contains(tfName))
                {
                    m_indexType.Add(tfName);
                    m_objectList.Add(childTrans);
                    m_nameList.Add(childTrans.name);
                }
                else if (childTrans.name.Contains(btnName))
                {
                    m_indexType.Add(btnName);
                    m_objectList.Add(childTrans.GetComponent<Button>());
                    m_nameList.Add(childTrans.name);
                }
                else if (childTrans.name.Contains(inputText))
                {
                    m_indexType.Add(inputText);
                    m_objectList.Add(childTrans.GetComponent<InputField>());
                    m_nameList.Add(childTrans.name);
                }
                else if (childTrans.name.Contains(rectName))
                {
                    m_indexType.Add(rectName);
                    m_objectList.Add(childTrans.GetComponent<RectTransform>());
                    m_nameList.Add(childTrans.name);
                    //sb.Append($"        {childTrans.name} = m_obj.transform.Find(\"{tmp}/{childTrans.name}\").GetComponent<RectTransform>();\n"); 
                }else if (childTrans.name.Contains(itemName))
                {
                    m_indexType.Add(itemName);
                    m_objectList.Add(childTrans.gameObject);
                    m_nameList.Add(childTrans.name);
                }
            }
        }

        private static void WriteMethod(GameObject gameObject, RefRoot refRoot, StringBuilder sb)
        {
            if (gameObject.GetComponent<RefRoot>() != null)
            {
                Object.Destroy(gameObject.GetComponent<RefRoot>());
            } 
            for (int i = 0; i < m_indexType.Count; i++)
            {
                if (m_indexType[i].Contains(" "))
                {
                    continue;
                }
                refRoot.AddRef(m_objectList[i]);
                if (m_indexType[i].Contains(textName))
                {
                    sb.Append($"          {m_nameList[i]} = refRoot.GetText({i});\n"); 
                }
                else if (m_indexType[i].Contains(imageName))
                {
                    sb.Append($"          {m_nameList[i]} = refRoot.GetImage({i});\n"); 
                }
                else if (m_indexType[i].Contains(goName))
                {
                    sb.Append($"          {m_nameList[i]} = refRoot.GetGameObject({i});\n"); 
                }
                else if (m_indexType[i].Contains(tfName))
                {
                    sb.Append($"         {m_nameList[i]} = refRoot.GetTransform({i});\n"); 
                }
                else if (m_indexType[i].Contains(btnName))
                {
                    sb.Append($"          {m_nameList[i]} = refRoot.GetButton({i});\n"); 
                }
                else if (m_indexType[i].Contains(inputText))
                {
                    sb.Append($"          {m_nameList[i]} = refRoot.GetInputField({i});\n"); 
                }
                else if (m_indexType[i].Contains(rectName))
                {
                    sb.Append($"          {m_nameList[i]} = refRoot.GetRectTransform({i});\n"); 
                }else if (m_indexType[i].Contains(itemName))
                {
                    sb.Append($"          {m_nameList[i]} = refRoot.GetGameObject({i});\n");
                }
            }
        }
    }
}