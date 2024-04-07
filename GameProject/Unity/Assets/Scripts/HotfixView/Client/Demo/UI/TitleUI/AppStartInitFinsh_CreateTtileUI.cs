namespace ET.Client
{
    [Event(SceneType.Demo)]
    public class AppStartInitFinsh_CreateTtileUI : AEvent<Scene,AppStartInitFinish>
    {
        protected override async ETTask Run(Scene scene, AppStartInitFinish args)
        {
            await UIHelper.Create(scene, UIType.TitleUI, UILayer.Mid);
        }
    }
}