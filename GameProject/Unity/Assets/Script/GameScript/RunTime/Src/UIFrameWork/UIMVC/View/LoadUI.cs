using GameTimer;
using MyGame;
using UnityEngine;
using UnityEngine.UI;

namespace MyGame
{
    public class LoadUI : UIWindow
    {
        #region 自动生成

        public Text m_textLoadText;
        public RectTransform m_rectFill;
        public RectTransform m_rectBackground;
        public GameObject m_goProgress;

        public override void OnUIAwake()
        {
            base.OnUIAwake();
            m_textLoadText = GameObject.Find("Bg/m_goProgress/m_textLoadText").GetComponent<Text>();
            m_rectFill = GameObject.Find("Bg/m_goProgress/m_rectBackground/m_rectFill").GetComponent<RectTransform>();
            m_rectBackground = GameObject.Find("Bg/m_goProgress/m_rectBackground").GetComponent<RectTransform>();
            m_goProgress = GameObject.Find("Bg/m_goProgress").gameObject;
        }

        #endregion



        private readonly string timerSource = "LoadUIUpdateDotText";

        public override void OnUIStart()
        {
            base.OnUIStart();
            GameTimerManager.CreateLoopFrameTimer(timerSource, 0.3f, UpdateTxt);
        }

        public override void OnUIDestroy()
        {
            OnUIDestroy();
            if (!GameTimerManager.IsFreeTimer(timerSource))
            {
                GameTimerManager.FreeTimer(timerSource);
            }
        }

        private int dotCnt = 0;
        private string[] dotStr = { "", ".", "..", "..." };

        private void UpdateTxt()
        {
            dotCnt++;
            if (dotCnt > 3)
            {
                dotCnt = 0;
            }

            string loadText = TextManager.GetText("玩命加载中");
            var tmpPtr = dotStr[dotCnt];
            m_textLoadText.text = $"{loadText}{tmpPtr}";
        }

        public override bool OnUIDestroyIsDestroy()
        {
            return false;
        }

        public void SetProgress(float progress)
        {
            float width = m_rectBackground.rect.width;
            float height = m_rectBackground.rect.height;
            float curWidth = width * progress;
            m_rectFill.sizeDelta = new Vector2(curWidth, height);
        }
    }
}
