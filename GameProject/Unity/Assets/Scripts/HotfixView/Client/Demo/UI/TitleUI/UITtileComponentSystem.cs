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
            self.MbtnEnter.GetComponent<Button>().onClick.AddListener(() => { self.EnterClick();});
        }

        private static void EnterClick(this UITitleComponent self)
        {
            
        }
    }
}

