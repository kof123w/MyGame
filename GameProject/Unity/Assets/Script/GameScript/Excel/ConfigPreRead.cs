using System;
using MyGame;

namespace Config
{  
    //预读取下配置，防止运行时卡顿
    public static class ConfigPreRead
    {  
        public static void PreRead()
        {
            DLogger.Log("==============PreRead Config============");
            Init(DefInputConfigMgr.Instance);
        }

        private static void Init(Object obj)
        {
        }
    }
}