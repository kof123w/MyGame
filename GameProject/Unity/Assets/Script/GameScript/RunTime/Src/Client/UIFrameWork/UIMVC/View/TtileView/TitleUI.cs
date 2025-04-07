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
            RefRoot refRoot = MGameObject.GetComponent<RefRoot>();
            m_textStart = refRoot.GetText(0);
            m_btnStart = refRoot.GetButton(1);
            m_inputAccount = refRoot.GetInputField(2);
            m_inputPassword = refRoot.GetInputField(3);
            Object.DestroyImmediate(refRoot);
        }
        #endregion

        public override void OnStart()
        {
            base.OnStart();
            m_btnStart.onClick.AddListener(OnClickStart);
        }

        private void OnClickStart()
        {
            // todo 没有数据功能，临时直接进入map01场景 
            GameEvent.Push(TaskEvent.TaskChange,typeof(SceneMap01Task));
            UIManager.Close<TitleUI>();
        }
    }
}