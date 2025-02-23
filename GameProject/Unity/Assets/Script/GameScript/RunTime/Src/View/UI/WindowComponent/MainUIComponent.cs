using UnityEngine;
using UnityEngine.UI;

namespace MyGame
{
    public class MainUIComponent : WindowComponent
    {
        private Text m_textStart;
        private Button m_btnStart;
        private InputField m_inputFieldAccount;
        private InputField m_inputFieldPassword;
        private GameObject m_obj;
        public void LoadCall(GameObject obj)
        {
            m_obj = obj;
            m_textStart = m_obj.transform.Find("MainWindow/Image/m_btnStart/m_textStart").GetComponent<Text>();
            m_btnStart = m_obj.transform.Find("MainWindow/Image/m_btnStart").GetComponent<Button>();
            m_inputFieldAccount = m_obj.transform.Find("MainWindow/Image/m_inputFieldAccount").GetComponent<InputField>();
            m_inputFieldPassword = m_obj.transform.Find("MainWindow/Image/m_inputFieldPassword").GetComponent<InputField>();
        }


    }
}