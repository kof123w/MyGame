using UnityEngine;
using UnityEngine.UI;

namespace MyGame
{
    [SystemOfComponent(typeof(MainUIComponent))]
    public class MainUISystem : ComponentSystem,IStart
    {
        public void Start(ref ComponentData data)
        {
            if(data is MainUIComponent window)
            {
                RefRoot refRoot = window.GetGameObject().GetComponent<RefRoot>();
                window.m_textStart = refRoot.GetText(0);
                window.m_btnStart = refRoot.GetButton(1);
                window.m_inputFieldAccount = refRoot.GetInputField(2);
                window.m_inputFieldPassword = refRoot.GetInputField(3);
            }
        }

    }
}