using UnityEngine.UI;
using UnityEngine;

namespace MyGame
{
    public class FrameSortUI : UIWindow
    {
        #region 自动生成
        public Text m_textConfirmedFrame;
        public override void OnUIAwake()
        {
            base.OnUIAwake();
            m_textConfirmedFrame = GameObject.Find("Bg/m_textConfirmedFrame").GetComponent<Text>();
        }
        #endregion

        public override void OnUIUpdate()
        {
            var syncFrame = FrameContext.Context.GetSyncFrame();
            m_textConfirmedFrame.text = $"SyncFrame:{syncFrame}";
        }
    }
}