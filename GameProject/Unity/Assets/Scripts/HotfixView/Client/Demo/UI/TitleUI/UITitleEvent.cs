using UnityEngine;
namespace ET.Client
{
    [UIEvent(UIType.TitleUI)]
    public class UITitleEvent : AUIEvent
    {
        public override async ETTask<UI> OnCreate(UIComponent uiComponent, UILayer uiLayer)
        {
            string assetsName = $"Assets/Bundles/UI/{UIType.TitleUI}.prefab";
            GameObject bundleGameObject = await uiComponent.Scene().GetComponent<ResourcesLoaderComponent>().LoadAssetAsync<GameObject>(assetsName);
            GameObject gameObject = UnityEngine.Object.Instantiate(bundleGameObject, uiComponent.UIGlobalComponent.GetLayer((int)uiLayer));
            UI ui = uiComponent.AddChild<UI, string, GameObject>(UIType.TitleUI, gameObject);
            ui.AddComponent<UITitleComponent>();
            return ui;
        }

        public override void OnRemove(UIComponent uiComponent)
        { 
        }
    }
}