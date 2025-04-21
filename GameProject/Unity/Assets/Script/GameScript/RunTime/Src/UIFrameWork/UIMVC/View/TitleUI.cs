using EventSystem;
using UnityEngine;
using UnityEngine.UI;
namespace MyGame
{
    class TitleUI : UIWindow
    {
        #region 自动生成
        public Text m_textStart;
        public Button m_btnStart;
        public InputField m_inputAccount;
        public InputField m_inputPassword;
        public override void OnAwake()
        {
            base.OnAwake();
            m_textStart = GameObject.Find("Bg/m_btnStart/m_textStart").GetComponent<Text>();
            m_btnStart = GameObject.Find("Bg/m_btnStart").GetComponent<Button>();
            m_inputAccount = GameObject.Find("Bg/m_inputAccount").GetComponent<InputField>();
            m_inputPassword = GameObject.Find("Bg/m_inputPassword").GetComponent<InputField>();
        }
        #endregion 

        public override void OnStart()
        {
            base.OnStart();
            m_btnStart.onClick.AddListener(OnClickStart);
        }

        private void OnClickStart()
        {
            var account = m_inputAccount.text;
            GameEvent.Push(TitleUIEvent.UIEventTitleUILogin, account);
        }
    }
}