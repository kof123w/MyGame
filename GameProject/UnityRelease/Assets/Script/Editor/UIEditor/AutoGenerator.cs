using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace MyGameEditor
{
    public class AutoGenerator
    {
        private static string m_TextName = "m_text";
        private static string m_ImageName = "m_img";
        private static string m_GoName = "m_go";
        private static string m_tfName = "m_tf";
        private static string m_BtnName = "m_btn";
        private static string m_InputText = "m_input";
        private static string m_rectName = "m_rect";

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
                sb.Append($"    public string m_uiName = \"{go.name}\";\n");
                WriteField(tf, sb);
                GUIUtility.systemCopyBuffer = sb.ToString();
            }
        }

        [MenuItem("GameObject/UI/AutoMethodGenerator")]
        private static void GenerateUIMethod()
        {
            Object activeObject = Selection.activeObject;
            if (activeObject is GameObject)
            {
                m_indexType.Clear();
                m_objectList.Clear();
                m_nameList.Clear();
                StringBuilder sb = new StringBuilder();
                GameObject go = activeObject as GameObject;
                Transform tf = go.transform;
                CalcTypeList(tf);
                sb.Append("    public void Awake(ref ComponentData data)\n");
                sb.Append("    {\n"); 
                sb.Append($"    if(data is {go.name} window)\n"); 
                sb.Append("     {\n");
                sb.Append("      RefRoot refRoot = window.GetGameObject().GetComponent<RefRoot>();\n");
                WriteMethod(go, sb);
                sb.Append("     }\n");
                sb.Append("    }\n");
                GUIUtility.systemCopyBuffer = sb.ToString();
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
                WriteField(childTrans, sb);
                if (childTrans.name.Contains(m_TextName))
                {
                    sb.Append($"    public Text {childTrans.name};\n");
                }
                else if (childTrans.name.Contains(m_ImageName))
                {
                    sb.Append($"    public Image {childTrans.name};\n");
                }
                else if (childTrans.name.Contains(m_GoName))
                {
                    sb.Append($"    public GameObject {childTrans.name};\n");
                }
                else if (childTrans.name.Contains(m_tfName))
                {
                    sb.Append($"    public Transform {childTrans.name};\n");
                }
                else if (childTrans.name.Contains(m_BtnName))
                {
                    sb.Append($"    public Button {childTrans.name};\n");
                }
                else if (childTrans.name.Contains(m_InputText))
                {
                    sb.Append($"    public InputField {childTrans.name};\n");
                }
                else if (childTrans.name.Contains(m_rectName))
                {
                    sb.Append($"    public RectTransform {childTrans.name};\n");
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
                CalcTypeListStack(childTrans);
                if (childTrans.name.Contains(m_TextName))
                {
                    m_indexType.Add(m_TextName);
                    m_objectList.Add(childTrans.GetComponent<Text>());
                    m_nameList.Add(childTrans.name);
                }
                else if (childTrans.name.Contains(m_ImageName))
                {
                    m_indexType.Add(m_ImageName);
                    m_objectList.Add(childTrans.GetComponent<Image>());
                    m_nameList.Add(childTrans.name);
                }
                else if (childTrans.name.Contains(m_GoName))
                {
                    m_indexType.Add(m_GoName);
                    m_objectList.Add(childTrans.gameObject);
                    m_nameList.Add(childTrans.name);
                }
                else if (childTrans.name.Contains(m_tfName))
                {
                    m_indexType.Add(m_tfName);
                    m_objectList.Add(childTrans);
                    m_nameList.Add(childTrans.name);
                }
                else if (childTrans.name.Contains(m_BtnName))
                {
                    m_indexType.Add(m_BtnName);
                    m_objectList.Add(childTrans.GetComponent<Button>());
                    m_nameList.Add(childTrans.name);
                }
                else if (childTrans.name.Contains(m_InputText))
                {
                    m_indexType.Add(m_InputText);
                    m_objectList.Add(childTrans.GetComponent<InputField>());
                    m_nameList.Add(childTrans.name);
                }
                else if (childTrans.name.Contains(m_rectName))
                {
                    m_indexType.Add(m_rectName);
                    m_objectList.Add(childTrans.GetComponent<RectTransform>());
                    m_nameList.Add(childTrans.name);
                    //sb.Append($"        {childTrans.name} = m_obj.transform.Find(\"{tmp}/{childTrans.name}\").GetComponent<RectTransform>();\n"); 
                }
            }
        }

        private static void WriteMethod(GameObject gameObject, StringBuilder sb)
        {
            if (gameObject.GetComponent<RefRoot>() != null)
            {
                Object.Destroy(gameObject.GetComponent<RefRoot>());
            }

            RefRoot refRoot = gameObject.AddComponent<RefRoot>();
            for (int i = 0; i < m_indexType.Count; i++)
            {
                refRoot.AddRef(m_objectList[i]);
                if (m_indexType[i].Contains(m_TextName))
                {
                    sb.Append($"          window.{m_nameList[i]} = refRoot.GetText({i});\n"); 
                }
                else if (m_indexType[i].Contains(m_ImageName))
                {
                    sb.Append($"          window.{m_nameList[i]} = refRoot.GetImage({i});\n"); 
                }
                else if (m_indexType[i].Contains(m_GoName))
                {
                    sb.Append($"          window.{m_nameList[i]} = refRoot.GetGameObject({i});\n"); 
                }
                else if (m_indexType[i].Contains(m_tfName))
                {
                    sb.Append($"          window.{m_nameList[i]} = refRoot.GetTransform({i});\n"); 
                }
                else if (m_indexType[i].Contains(m_BtnName))
                {
                    sb.Append($"          window.{m_nameList[i]} = refRoot.GetButton({i});\n"); 
                }
                else if (m_indexType[i].Contains(m_InputText))
                {
                    sb.Append($"          window.{m_nameList[i]} = refRoot.GetInputField({i});\n"); 
                }
                else if (m_indexType[i].Contains(m_rectName))
                {
                    sb.Append($"          window.{m_nameList[i]} = refRoot.GetRectTransform({i});\n"); 
                }
            }
        }
    }
}