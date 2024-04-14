namespace ET.Client
{
    public class GameSceneManager : Singleton<GameSceneManager>
    {
        public GameSceneType CurGameScene { private set; get; }


        //切换场景
        public void ChangeScene(GameSceneType gameSceneType)
        {
            this.CurGameScene = gameSceneType;
        }
    }
}

