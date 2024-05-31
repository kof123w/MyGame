using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Config;
public class GameLogic 
{
     public static void HotTest()
     {
          var configObject = TestConfigMgr.Instance.GetTestConfigConfig(1);
          Debug.Log("打印下配置表:" + configObject.Name );
          Debug.Log("测试下热更新！我是热更新后的打印。");
     }
}
