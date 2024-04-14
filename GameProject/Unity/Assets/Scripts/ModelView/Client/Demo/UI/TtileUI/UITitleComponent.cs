using UnityEngine;
using UnityEngine.UI;

namespace ET.Client
{
    [ComponentOf(typeof(UI))]
    public class UITitleComponent : Entity,IAwake
    {
        public GameObject MbtnEnter;
        public GameObject MSelectPlayMode;
        public GameObject MBtnOffLine;
        public GameObject MbtnOnLine;
        public GameObject MBtnBack;
        public GameObject MGoLogUI;
        public GameObject MAccount;
        public GameObject MPassword;
        public GameObject MLogin;

        public InputField Account;
        public InputField Password;

        public bool IsInOnLinePage = false;
        public bool IsSelect = false;
    }
}

