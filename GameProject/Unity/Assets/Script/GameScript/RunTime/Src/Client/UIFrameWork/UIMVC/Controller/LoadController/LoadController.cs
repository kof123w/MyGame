using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MyGame
{
    [ControllerOf(typeof(LoadModel))]
    public class LoadController : UIController
    { 
        public override void InitController()
        { 
            this.Subscribe<float>(UIEvent.UIEventLoadUISetProgress,SetUIProgress); 
        }

        private async void SetUIProgress(float progress)
        {  
            var loadUI = await UIManager.GetWindow<LoadUI>();
            loadUI.SetProgress(progress);
        }
    }
}
