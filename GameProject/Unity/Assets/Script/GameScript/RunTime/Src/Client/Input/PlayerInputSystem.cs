using System;
using System.Collections.Generic;
using Config;
using UnityEngine; 

namespace MyGame
{
    public class PlayerInputSystem : Singleton<PlayerInputSystem>
    {
        private List<DefInputConfig> defInputConfigs;

        private Dictionary<int, KeyCode> cmdKeyMap = new();

        public void Init()
        {
            DLogger.Log("==============>Init Input Module");
            defInputConfigs = DefInputConfigManager.Instance.GetInputConfigs();
            if (defInputConfigs != null)
            {
                for (int i = 0; i < defInputConfigs.Count; i++)
                {
                    foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
                    { 
                        if (keyCode.ToString().Equals(defInputConfigs[i].Keys))
                        {
                            cmdKeyMap.Add(defInputConfigs[i].ID,keyCode);
                        }
                    } 
                }
            }
        }

        public void Tick()
        {
            RevInput();
        }

        //接收输入
        public void RevInput()
        {
            if (defInputConfigs != null)
            {
                if (Input.anyKey)
                {
                    for (int i = 0; i < defInputConfigs.Count; i++)
                    {
                        if (cmdKeyMap.ContainsKey(defInputConfigs[i].ID))
                        {
                            var keyCode = cmdKeyMap[defInputConfigs[i].ID];
                            if (Input.GetKey(keyCode))
                            {
                                GameEvent.Push(InputEvent.KeyHold,(InputSignal)defInputConfigs[i].ID);
                                Debug.Log(keyCode.ToString());
                            }
                        }
                    }
                } 
                
                if (Input.anyKeyDown)
                {
                    for (int i = 0; i < defInputConfigs.Count; i++)
                    {
                        if (cmdKeyMap.ContainsKey(defInputConfigs[i].ID))
                        {
                            var keyCode = cmdKeyMap[defInputConfigs[i].ID];
                            if (Input.GetKeyDown(keyCode))
                            {
                                GameEvent.Push(InputEvent.KeyHold,(InputSignal)defInputConfigs[i].ID);
                                Debug.Log(keyCode.ToString());  
                            }
                        }
                    }
                }   
            }
        } 
    }
}