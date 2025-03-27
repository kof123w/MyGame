using System;
using System.Collections.Generic;
using Config;
using UnityEngine;

namespace MyGame
{
    public class InputMgr : Singleton<InputMgr>
    {
        private List<DefInputConfig> m_defInputConfigs;

        private Dictionary<int, KeyCode> m_cmdKeyMap = new();
        //初始化
        public void Init()
        {
            DLogger.Log("===========Init Input Module==========");
            m_defInputConfigs = DefInputConfigManager.Instance.GetInputConfigs();
            if (m_defInputConfigs != null)
            {
                for (int i = 0; i < m_defInputConfigs.Count; i++)
                {
                    foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
                    { 
                        if (keyCode.ToString().Equals(m_defInputConfigs[i].Keys))
                        {
                            m_cmdKeyMap.Add(m_defInputConfigs[i].ID,keyCode);
                        }
                    } 
                }
            }
        }

        //接收输入
        public void RevInput()
        {
            if (m_defInputConfigs != null)
            {
                if (Input.anyKey)
                {
                    for (int i = 0; i < m_defInputConfigs.Count; i++)
                    {
                        if (m_cmdKeyMap.ContainsKey(m_defInputConfigs[i].ID))
                        {
                            var keyCode = m_cmdKeyMap[m_defInputConfigs[i].ID];
                            if (Input.GetKey(keyCode))
                            {
                                this.Push<InputCmd>(InputEvent.KeyHold,(InputCmd)m_defInputConfigs[i].ID);
                                Debug.Log(keyCode.ToString());
                            }
                        }

                    }
                } 
                
                if (Input.anyKeyDown)
                {
                    for (int i = 0; i < m_defInputConfigs.Count; i++)
                    {
                        if (m_cmdKeyMap.ContainsKey(m_defInputConfigs[i].ID))
                        {
                            var keyCode = m_cmdKeyMap[m_defInputConfigs[i].ID];
                            if (Input.GetKey(keyCode))
                            {
                                this.Push<InputCmd>(InputEvent.KeyHold,(InputCmd)m_defInputConfigs[i].ID);
                                Debug.Log(keyCode.ToString());
                            }
                        }
                    }
                }   
            }

           
        }
    }
}