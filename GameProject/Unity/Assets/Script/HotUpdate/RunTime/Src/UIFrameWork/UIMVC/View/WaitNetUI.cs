using MyGame;
using UnityEngine;
using UnityEngine.UI;

namespace MyGame
{
    public class WaitNetUI : UIWindow
    {
        #region 自动生成

        public Text m_textLoadText;
        public GameObject m_goProgress;

        public override void OnUIAwake()
        {
            base.OnUIAwake();
            m_textLoadText = GameObject.Find("Bg/m_goProgress/m_textLoadText").GetComponent<Text>();
            m_goProgress = GameObject.Find("Bg/m_goProgress").gameObject;
        }

        #endregion

        public override bool OnUIDestroyIsDestroy()
        {
            return false;
        } 
    }
}
