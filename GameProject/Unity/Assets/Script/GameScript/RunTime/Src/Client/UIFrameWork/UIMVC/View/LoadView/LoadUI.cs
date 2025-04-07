using MyGame;
using UnityEngine;
using UnityEngine.UI; 

public class LoadUI : UIWindow
{
    #region 自动生成
    public Text m_textLoadText;
    public RectTransform m_rectFill;
    public RectTransform m_rectBackground;
    public GameObject m_goProgress;
    public override void OnAwake()
    {
        base.OnAwake();
        RefRoot refRoot = MGameObject.GetComponent<RefRoot>();
        m_textLoadText = refRoot.GetText(0);
        m_rectFill = refRoot.GetRectTransform(1);
        m_rectBackground = refRoot.GetRectTransform(2);
        m_goProgress = refRoot.GetGameObject(3);
    }
    #endregion 
    private readonly string timerSource = "LoadUIUpdateDotText";
    public override void OnStart()
    {
        base.OnStart();
        GameTimerManager.CreateLoopFrameTimer(timerSource,0.3f,UpdateTxt);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        if (!GameTimerManager.IsFreeTimer(timerSource))
        {
            GameTimerManager.FreeTimer(timerSource);
        }
    }

    private int dotCnt = 0;
    private string[] dotStr = {"",".","..","..."};
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

    public override bool OnDestroyIsDestroy()
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
