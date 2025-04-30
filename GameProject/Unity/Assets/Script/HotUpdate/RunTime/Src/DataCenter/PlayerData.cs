using EventSystem;
using SingleTool;

namespace MyGame
{
    public class PlayerData : DataClass
    {
        private string account;
        private long playerId;

        public override void InitEvent()
        {
            this.Subscribe<string,long>(PlayerDataEvent.PlayerDataEventSetData,SetData);
        }

        public override void OutLogin()
        {
            account = null;
            playerId = 0;
        }

        public void SetData(string accountParam, long playerIdParam)
        {
            account = accountParam;
            playerId = playerIdParam;
        }

        public long GetId()
        {
            return playerId;
        }
    }
}