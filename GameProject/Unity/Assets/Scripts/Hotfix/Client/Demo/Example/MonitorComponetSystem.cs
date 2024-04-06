namespace ET.Client{[EntitySystemOf(typeof(MonitorComponent))]
    [FriendOf(typeof(MonitorComponent))]
    public static partial class MonitorComponetSystem
    {
        [EntitySystem]
        private static void Awake(this ET.Client.MonitorComponent self, int args2)
        {
            Log.Debug("Monitor Component System Awake!");
            self.Brightness = args2;
        }
    
    
        [EntitySystem]
        private static void Destroy(this ET.Client.MonitorComponent self)
        {
            Log.Debug("Monitor Component System Destory!");
        }

        public static void ChangeBrightness(this ET.Client.MonitorComponent self,int value)
        {
            self.Brightness = value;
        }
    }}

