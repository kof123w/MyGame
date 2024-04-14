using UnityEngine;

namespace ET.Client
{
    [Event(SceneType.Demo)]
    public  class EnterScene_RemoveUI : AEvent<Scene,EnterScene>
    {
        protected override async ETTask Run(Scene scene, EnterScene a)
        {
            await UIHelper.Remove(scene, UIType.TitleUI); 
        }
    }
}

