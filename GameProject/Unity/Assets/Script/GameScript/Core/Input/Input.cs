using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
    public class Input : Singleton<Input>
    {
        //按照InputConst顺序来
        private List<KeyCode> m_keyCode = new List<KeyCode>();
        //初始化
        public void Init()
        {
            DLogger.Log("===========Init Input Module==========");
            
            
        }
    }
}