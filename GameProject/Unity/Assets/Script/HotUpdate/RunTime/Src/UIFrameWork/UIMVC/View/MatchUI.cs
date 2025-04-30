using EventSystem;
using UnityEngine;
using UnityEngine.UI;

namespace MyGame
{
    public class MatchUI : UIWindow
    {
        #region 自动生成
        public Button m_btnMatch;
        public override void OnUIAwake()
        {
            base.OnUIAwake();
            m_btnMatch = GameObject.Find("Bg/m_btnMatch").GetComponent<Button>();
        }
        #endregion

        public override void OnUIStart()
        {
            base.OnUIStart();
            m_btnMatch.onClick.AddListener(Match);
        }

        private void Match()
        {
            // start Match
            GameEvent.Push(NetEvent.MatchHandleEvent);
        }
    }
}