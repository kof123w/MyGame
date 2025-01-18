namespace MyGame
{
    public static class GameWorldConst
    {
        public static long GameWorldAwakeEventId = "GameWorld.Awake".StringToHash();
        public static long GameWorldEnableEventId = "GameWorld.OnEnable".StringToHash();
        public static long GameWorldDisableEventId = "GameWorld.Disable".StringToHash();
        public static long GameWorldStartEventId = "GameWorld.Start".StringToHash();
        public static long GameWorldDestoryEventId = "GameWorld.Destroy".StringToHash();
    }
}

