using UnityEngine;

namespace MyGame
{ 
    public class UIWindow : WindowBase
    {
        protected int MSort;
        protected Canvas MCanvas;
        
        public void SetWindowSortingOrder(ref int sort)
        {
            MSort = sort;
        }

        public void GetWindowSortingOrder(out int sort)
        {
            sort = MSort;
        }

        public void LoadUIResource()
        {
            //ResourceLoader.Instance.LoadUIResource(UIManager.GetUIRoot(),callback);
        }
        
        
    }
}