using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using EventSystem;
using UnityEngine; 

namespace MyGame
{ 
    public class SceneMap01Task : ITask
    { 
        public async UniTaskVoid Run()
        {
            await UIManager.Show<LoadUI>();  
            GameWorld.Instance.ChangeScene(SceneType.Map01,out var previous,out var current); 
            if (previous != null)
            {
                previous.UnloadScene().Forget();
            }
            GameWorld.Instance.CreatePlayerRole(1); 
            PlayerCharacter playerCharacter = GameWorld.Instance.GetPlayerCharacter(); 
            LoadTaskSort loadTaskSort = new LoadTaskSort();
            //加载场景
            loadTaskSort.AddLoadTask(current.LoadScene(),current);
            //加载玩家
            loadTaskSort.AddLoadTask(playerCharacter.LoadActor(),playerCharacter);
            await loadTaskSort.WaitAllTasksAsync(); 
            await UniTask.Delay(1000); 
            await UniTask.Yield(PlayerLoopTiming.Update); 
            playerCharacter.SetWorldPos(new Vector3(0,10,0)); 
            current.Start();
            playerCharacter.Start();
            UIManager.Close<LoadUI>();
        }
    }
}