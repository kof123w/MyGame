using EventSystem; 

namespace MyGame
{
    public class TitleController : UIController
    { 
        public override void InitController()
        { 
            this.Subscribe(UIEvent.UIEventOpenTitleUI,OpenTitleUI); 
            this.Subscribe(UIEvent.UIEventCloseTitleUI,OpenTitleUI); 
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