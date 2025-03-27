using System;
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
            RefRoot refRoot = MGameObject.GetComponent<RefRoot>();
            m_textStart = refRoot.GetText(0);
            m_btnStart = refRoot.GetButton(1);
            m_inputAccount = refRoot.GetInputField(2);
            m_inputPassword = refRoot.GetInputField(3);
        }
        #endregion

        public override void OnStart()
        {
            base.OnStart();
            m_btnStart.onClick.AddListener(OnClickStart);
        }

        private void OnClickStart()
        {
            DLogger.Log("OnClickStart");
            if (!string.IsNullOrEmpty(m_inputAccount.text))
            {
                DLogger.Log($"m_inputAccount.text is {m_inputAccount.text}");
            }
            
            if (!string.IsNullOrEmpty(m_inputPassword.text))
            {
                DLogger.Log($"m_inputPassword.text is {m_inputPassword.text}");
            }
        }
    }
}