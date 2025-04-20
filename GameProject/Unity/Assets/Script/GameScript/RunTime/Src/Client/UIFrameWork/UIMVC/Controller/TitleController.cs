using DebugTool;
using EventSystem; 

namespace MyGame
{
    public class TitleController : UIController
    {
        public override void InitController()
        { 
            this.Subscribe(UIEvent.UIEventOpenTitleUI,OpenTitleUI); 
            this.Subscribe(UIEvent.UIEventCloseTitleUI,OpenTitleUI); 
            this.Subscribe<string>(UIEvent.UIEventTitleUILogin,Login);
        }

        private void Login(string account)
        {
             //var packet = await LoginHandler.Instance.LoginHandler_Login(account);
            GameEvent.Push(NetEvent.LoginHandleEvent, account);
        }

        private async void OpenTitleUI()
        {  
            await UIManager.Show<TitleUI>();
        }

        private async void CloseTitleUI()
        {
            UIManager.Close<TitleUI>();
        }
    }
}