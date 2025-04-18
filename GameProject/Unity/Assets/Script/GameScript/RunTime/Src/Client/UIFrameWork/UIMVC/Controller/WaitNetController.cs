using EventSystem;

namespace MyGame
{
    public class WaitNetController : UIController
    {
        public override void InitController()
        { 
            this.Subscribe(UIEvent.CloseWaitNetUI,CloseWaitNetUI); 
            this.Subscribe(UIEvent.OpenWaitNetUI,OpenWaitNetUI);
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