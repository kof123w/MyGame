namespace MyGame
{
    public class UIWindow : WindowBase
    {
        protected int MWindowSort = 0;

        public void SetWindowSort(ref int sort)
        {
            MWindowSort = sort;
        }

        public void GetWindowSort(out int sort)
        {
            sort = MWindowSort;
        }
    }
}