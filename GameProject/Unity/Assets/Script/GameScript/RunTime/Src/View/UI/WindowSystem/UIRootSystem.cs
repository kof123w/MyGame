using System;
using UnityEngine;

namespace MyGame
{
    [SystemOfComponent(typeof(UIRootComponent))]
    public class UIRootSystem : ComponentSystem,IStart
    {
        public void Start(ref ComponentData data)
        {
            if (data is UIRootComponent uiRootComponent)
            {
                uiRootComponent.UIRoot = GameObject.Find("UIFramework/UIRoot").transform; 
                //注册下
                this.Subscribe<Type>(uiRootComponent.EventOpenWindowEventID,OpenWindow);
                this.Subscribe<Type,WindowComponent>(uiRootComponent.EventCloseWindowEventID,CloseWindow);
            }
        }

        private void OpenWindow(Type windowType)
        {
            //
            var windowEntity = GameWorld.Instance.WindowEntityObject;
            var componentData = windowEntity.AddComponentByType(windowType);
            if (componentData != null)
            { 
                Action<GameObject> callBack = null;
                
                var uIRoot = GameWorld.Instance.UIRootComponentObject.UIRoot;
                ResourceLoader.Instance.LoadUIResource(windowType,uIRoot,callBack,windowType.Name); 
            } 
        } 

        private void CloseWindow(Type windowType,WindowComponent component)
        {
            //var system = GameWorld.Instance.EntityManager.GetComponentSystem(windowType);
            var windowEntity = GameWorld.Instance.WindowEntityObject;
            windowEntity.RemoveComponentByType(windowType);
            
            ResourceLoader.Instance.DestroyUIResource(component.GetGameObject());
        }
    }
}