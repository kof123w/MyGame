﻿using System;
using DebugTool;
using MyGame;

namespace Config
{  
    //预读取下配置，防止运行时卡顿
    public static class ConfigPreRead
    {  
        public static void PreRead()
        {
            DLogger.Log("==============>PreRead Config");
            Init(DefInputConfigManager.Instance);
            Init(ResourceConfigManager.Instance);
            Init(RoleBaseAttributeConfigManager.Instance);
        }

        private static void Init(Object obj)
        {
            //TODO..
        }
    }
}