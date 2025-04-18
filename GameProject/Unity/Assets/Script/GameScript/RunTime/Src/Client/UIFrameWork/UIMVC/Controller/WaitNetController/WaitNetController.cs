using EventSystem;

namespace MyGame
{
    public class WaitNetController : UIController
    {
        public override void InitController()
        { 
            this.Subscribe(NetEvent.CloseWaitNetUI,CloseWaitNetUI); 
            this.Subscribe(NetEvent.OpenWaitNetUI,OpenWaitNetUI);
        }

        private void CloseWaitNetUI()
        {  
            UIManager.Close<WaitNetUI>();
        }

        public async void OpenWaitNetUI()
        {
             await UIManager.Show<WaitNetUI>(); 
        }
    }
}