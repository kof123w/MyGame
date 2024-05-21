using UnityEngine;
using System.IO;
using System.Reflection;

class StartGame : UnitySingleton<StartGame>
{
    void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        //初始化框架参数
        // todo
        
        //资源管理..版本更新
        // todo
        
        //进入游戏 
        Assembly hotUpdateAss = Assembly.Load(File.ReadAllBytes($"Library//ScriptAssemblies//Framework.Core.dll.bytes"));
        var type = hotUpdateAss.GetType("GameLogic");
        type.GetMethod("HotTest").Invoke(null, null);
    }
}
