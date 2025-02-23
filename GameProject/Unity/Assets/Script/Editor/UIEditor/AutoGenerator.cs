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
        
        [MenuItem("GameObject/UI/AutoGenerator")]
        private static void GenerateUI()
        {
            Object activeObject = Selection.activeObject;
            if (activeObject is GameObject)
            {
                StringBuilder sb = new StringBuilder();
                GameObject go = activeObject as GameObject;
                Transform tf = go.transform;
                WriteField(tf,sb); 
                sb.Append("    private GameObject m_obj;\n");
                sb.Append("    public void LoadCall(GameObject obj)\n");
                sb.Append("    {\n");
                sb.Append($"    m_obj = obj;\n");
                WriteMethod(tf,sb);
                sb.Append("    }\n");
                GUIUtility.systemCopyBuffer = sb.ToString();
            }
        }

        private static void WriteField(Transform tf,StringBuilder sb)
        {
            if (tf.childCount <= 0)
            {
                return;
            }

            for (int i = 0; i < tf.childCount; i++)
            { 
                var childTrans = tf.GetChild(i);
                WriteField(childTrans,sb);
                if (childTrans.name.Contains(m_TextName))
                {
                    sb.Append($"    private Text {childTrans.name};\n");
                }
                else if (childTrans.name.Contains(m_ImageName))
                {
                    sb.Append($"    private Image {childTrans.name};\n");
                }
                else if (childTrans.name.Contains(m_GoName))
                {
                    sb.Append($"    private GameObject {childTrans.name};\n");
                }
                else if (childTrans.name.Contains(m_tfName))
                {
                    sb.Append($"    private Transform {childTrans.name};\n");
                }
                else if (childTrans.name.Contains(m_BtnName))
                {
                    sb.Append($"    private Button {childTrans.name};\n");
                }
                else if (childTrans.name.Contains(m_InputText))
                {
                    sb.Append($"    private InputField {childTrans.name};\n");
                }
            }
        }

        
        private static void WriteMethod(Transform tf, StringBuilder sb)
        {
            string pos = "";
            for (int i = 0; i < tf.childCount; i++)
            {
                WriteMethodStack(tf,pos,sb);
            } 
        }

        private static void WriteMethodStack(Transform tf, string pos,StringBuilder sb)
        {
            if (tf.childCount <= 0)
            {
                return;
            }
            
            string tmp = string.IsNullOrEmpty(pos) ? $"{tf.name}" : $"{pos}/{tf.name}";
            for (int i = 0; i < tf.childCount; i++) {
                var childTrans = tf.GetChild(i);
                WriteMethodStack(childTrans,tmp,sb);
                if (childTrans.name.Contains(m_TextName))
                { 
                    sb.Append($"        {childTrans.name} = m_obj.transform.Find(\"{tmp}/{childTrans.name}\").GetComponent<Text>();\n");
                }
                else if (childTrans.name.Contains(m_ImageName))
                {
                    sb.Append($"        {childTrans.name} = m_obj.transform.Find(\"{tmp}/{childTrans.name}\").GetComponent<Image>();\n");
                }
                else if (childTrans.name.Contains(m_GoName))
                {
                    sb.Append($"        {childTrans.name} = m_obj.transform.Find(\"{tmp}/{childTrans.name}\").gameObject;\n");
                }
                else if (childTrans.name.Contains(m_tfName))
                {
                    sb.Append($"        {childTrans.name} = m_obj.transform.Find(\"{tmp}/{childTrans.name}\");\n");
                } 
                else if (childTrans.name.Contains(m_BtnName))
                {
                    sb.Append($"        {childTrans.name} = m_obj.transform.Find(\"{tmp}/{childTrans.name}\").GetComponent<Button>();\n");
                } 
                else if (childTrans.name.Contains(m_InputText))
                {
                    sb.Append($"        {childTrans.name} = m_obj.transform.Find(\"{tmp}/{childTrans.name}\").GetComponent<InputField>();\n"); 
                }
            }
        }
    }
}
