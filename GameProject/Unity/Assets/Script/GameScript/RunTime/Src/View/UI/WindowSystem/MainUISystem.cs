using UnityEngine;

namespace MyGame
{
    [SystemOfComponent(typeof(MainUIComponent))]
    public class MainUISystem : ComponentSystem,IStart,ILoadCall
    {
        public void Start(ref ComponentData data)
        { 
            MainUIComponent mainUIComponent = (MainUIComponent)data;
            
        }

        public void LoadCall(GameObject gameObject)
        { 
            
        }
    }
}