namespace MyGame
{
    public interface ISaveDataManager
    {
        void SaveBin();

        void LoadBin();

        void CreateBin();

        void DeleteBin();
    }
}