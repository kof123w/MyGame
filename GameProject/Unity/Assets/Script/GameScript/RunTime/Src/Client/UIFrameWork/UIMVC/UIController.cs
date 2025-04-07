namespace MyGame
{
    public class UIController
    {
        protected UIModel UILogic;

        public void SetUIModel(UIModel uiLogic)
        {
            UILogic = uiLogic;
        }

        public virtual void InitController()
        {
            
        }
    }
}