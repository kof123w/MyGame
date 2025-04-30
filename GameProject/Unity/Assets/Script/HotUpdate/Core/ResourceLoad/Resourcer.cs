using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AssetsLoad
{
    //Use the basic assets-bundle solution (Async and Sync) 
    public class Resourcer : IResourcer
    {
        public void UnloadUnusedAssets()
        {
#if UNITY_EDITOR 
            Resources.UnloadUnusedAssets();
#else
            Resources.UnloadUnusedAssets();
#endif
        } 

        public async UniTask<Object> LoadAsync(string path,CancellationToken token, IProgress<float> progress = null)
        {
#if UNITY_EDITOR
            var request = Resources.LoadAsync<GameObject>(path); 
            var progressTask = Track(request,CancellationTokenSource.CreateLinkedTokenSource(token).Token, progress);
            var loadTask = request.ToUniTask(cancellationToken: token);
            await UniTask.WhenAny(loadTask, progressTask); 
            return request.asset;
#else
            var request = Resources.LoadAsync<GameObject>(path); 
            var progressTask = Track(request,CancellationTokenSource.CreateLinkedTokenSource(token).Token, progress);
            var loadTask = request.ToUniTask(cancellationToken: token);
            await UniTask.WhenAny(loadTask, progressTask); 
            return request.asset;
#endif
        }

        public async UniTask Track(ResourceRequest request, CancellationToken token, IProgress<float> progress = null)
        {
            while (!request.isDone && !token.IsCancellationRequested)
            {
                progress?.Report(request.progress);
                await UniTask.Yield(PlayerLoopTiming.Update,token);
            }

            // 最终报告100%进度（除非被取消）
            if (!token.IsCancellationRequested)
            {
                progress?.Report(1f);
            }
        }
    }
}