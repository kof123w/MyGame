using UnityEngine;
using UnityEngine.UI;

namespace MyGame
{
    public class MainUIComponent : WindowComponent
    {
        public string m_uiName = "MainUIComponent";
        public Text m_textStart;
        public Button m_btnStart;
        public InputField m_inputFieldAccount;
        public InputField m_inputFieldPassword; 
    }
}