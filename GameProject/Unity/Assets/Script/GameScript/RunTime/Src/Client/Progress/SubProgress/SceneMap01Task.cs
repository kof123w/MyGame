using System;
using Cysharp.Threading.Tasks;
using UnityEngine; 

namespace MyGame
{
    [ProgressLoopCheck(true)]
    public class SceneMap01Task : ITask
    {
        public async UniTaskVoid Run()
        {
            await UIManager.Show<LoadUI>();
            float totalProgress = 0;
            float p1 = 0;
            float p2 = 0;
            IProgress<float> sceneLoadTracker = new Progress<float>(p =>
            {
                p1 = p; 
                totalProgress = p1 * 0.5f + p2 * 0.5f;
                GameEvent.Push(UIEvent.UIEventLoadUISetProgress,totalProgress);
            });

            IProgress<float> playerLoadTracker = new Progress<float>(p =>
            {
                p2 = p; 
                totalProgress  = p1 * 0.5f + p2 * 0.5f;
                GameEvent.Push(UIEvent.UIEventLoadUISetProgress,totalProgress);
            });
            
            GameWorld.Instance.ChangeScene(SceneType.Map01,out var previous,out var current); 
            if (previous != null)
            {
                previous.UnloadScene().Forget();
            }
            current.SetTracker(sceneLoadTracker); 
            GameWorld.Instance.CreatePlayerRole(1); 
            PlayerCharacter playerCharacter = GameWorld.Instance.GetPlayerCharacter();
            playerCharacter.SetTracker(playerLoadTracker);
            //开始加载
           // await UniTask.WhenAll(current.LoadScene(),playerCharacter.LoadActor());
            await current.LoadScene();
            await UniTask.Delay(2000);
            await playerCharacter.LoadActor();
            await UniTask.Delay(2000);
            await UniTask.Yield(PlayerLoopTiming.Update);
            UIManager.Close<LoadUI>();
        }
    }
}