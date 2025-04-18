using EventSystem;
using SingleTool;

namespace MyGame
{
    public class PlayerDataManager : DataClass
    {
        private string account;
        private long playerId;

        public override void InitEvent()
        {
            this.Subscribe<string,long>(DataEvent.PlayerDataEventSetData,SetData);
        }

        public override void OutLogin()
        {
            account = null;
            playerId = 0;
        }

        private void SetData(string accountParam, long playerIdParam)
        {
            account = accountParam;
            playerId = playerIdParam;
        }
    }
}