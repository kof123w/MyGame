using UnityEngine;
using UnityEngine.UI;

namespace ET.Client
{
    [EntitySystemOf(typeof(UITitleComponent))]
    [FriendOf(typeof(UITitleComponent))]
    public static partial class UITitileComponentSystem
    {
        [EntitySystem]
        private static void Awake(this ET.Client.UITitleComponent self)
        {
            ReferenceCollector rc = self.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            self.MbtnEnter = rc.Get<GameObject>("m_btnEnterGame");
            self.MGoLogUI = rc.Get<GameObject>("m_goLogUI");
            self.MSelectPlayMode = rc.Get<GameObject>("m_selectPlayMode");
            self.MAccount = rc.Get<GameObject>("m_account");
            self.MPassword = rc.Get<GameObject>("m_password");
            self.MLogin = rc.Get<GameObject>("m_login");
            self.MBtnBack = rc.Get<GameObject>("m_btnBack");
            self.MbtnOnLine = rc.Get<GameObject>("m_btnOnLine");
            self.MBtnOffLine = rc.Get<GameObject>("m_btnOffLine");
            self.Account = self.MAccount.GetComponent<InputField>();
            self.Password = self.MPassword.GetComponent<InputField>();
            
            self.MbtnEnter.GetComponent<Button>().onClick.AddListener(() => { self.EnterClick();});
            self.MbtnOnLine.GetComponent<Button>().onClick.AddListener(() => { self.OnLineClick();});
            self.MBtnOffLine.GetComponent<Button>().onClick.AddListener(() => { self.OffLineClick();});
            self.MBtnBack.GetComponent<Button>().onClick.AddListener(() => { self.BackClick();});
            self.MLogin.GetComponent<Button>().onClick.AddListener(() => { self.LoginClick();});
            
            //初始化
            self.AllClose();
            self.MSelectPlayMode.SetActive(true);
        }

        private static void LoginClick(this UITitleComponent self)
        {
            Log.Debug("登录游戏!");
        }

        private static void OnLineClick(this UITitleComponent self)
        {
            self.IsSelect = true;
            self.IsInOnLinePage = true;
            self.AllClose();
            self.MGoLogUI.SetActive(true);
            self.MBtnBack.SetActive(true);
        }

        private static void OffLineClick(this UITitleComponent self)
        {
            self.IsSelect = true;
            self.IsInOnLinePage = false;
            self.AllClose();
            self.MbtnEnter.SetActive(true);
            self.MBtnBack.SetActive(true);
        }


        private static void BackClick(this UITitleComponent self)
        {
            self.IsSelect = false;
            self.IsInOnLinePage = false;
            self.AllClose();
            self.MSelectPlayMode.SetActive(true);
        }

        private static void EnterClick(this UITitleComponent self)
        {
            Log.Debug("离线进入游戏!");
        }

        private static void AllClose(this UITitleComponent self)
        {
            self.MbtnEnter.SetActive(false);
            self.MGoLogUI.SetActive(false);
            self.MSelectPlayMode.SetActive(false); 
            self.MBtnBack.SetActive(false); 
        }
    }
}

