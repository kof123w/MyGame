using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using EventSystem;

namespace MyGame
{
    public class LoadTaskSort
    {
        private List<UniTask> loadTasks = new List<UniTask>();
        private List<float> progressSorts = new List<float>();

        public void AddLoadTask(UniTask task,AssetShape assetShape)
        {
            int curHasElement = loadTasks.Count;
            IProgress<float> tracker = new Progress<float>((p) =>
            {
                int index = curHasElement;
                if (index < progressSorts.Count) {
                    progressSorts[index] = p;
                }
                else {
                    progressSorts.Add(p);
                }

                float totalProgress = 0;
                foreach (var t in progressSorts)
                {
                    totalProgress += t / progressSorts.Count;
                }
                
                GameEvent.Push(UIEvent.UIEventLoadUISetProgress, totalProgress); 
            });
            assetShape.SetTracker(tracker); 
            loadTasks.Add(task);
        }

        public async UniTask WaitAllTasksAsync()
        { 
            await UniTask.WhenAll(loadTasks);
        }
    }
}