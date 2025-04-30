namespace MyGame
{
    public interface ISaveDataBase
    {
        public long Id { get; set; }

        public void Save();

        public void Init();

        public void Load();
    }
}


