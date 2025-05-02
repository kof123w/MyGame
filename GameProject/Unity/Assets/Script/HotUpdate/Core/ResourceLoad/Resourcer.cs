using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace AssetsLoad
{
    //Use the basic assets-bundle solution (Async and Sync) 
    public class Resourcer : IResourcer
    {
        public void UnloadUnusedAssets()
        {
#if UNITY_LOCAL_SCRIPT
            Resources.UnloadUnusedAssets();
#else
            Resources.UnloadUnusedAssets();
#endif
        }

        public async UniTask<TextAsset> LoadConfigAsset(string assetName)
        {
            var asset = Addressables.LoadAssetAsync<TextAsset>(assetName);
            return await asset;
        }

        public async UniTask<Object> LoadAsync(string resName, CancellationToken token,
            IProgress<float> progress = null)
        {
            var request = Addressables.LoadAssetAsync<GameObject>(resName);
            //var request = Resources.LoadAsync<GameObject>(resName); 
            var progressTask = Track(request, CancellationTokenSource.CreateLinkedTokenSource(token).Token, progress);
            var loadTask = request.ToUniTask(cancellationToken: token);
            await UniTask.WhenAny(loadTask, progressTask);
            return request.Result;
        }

        public async UniTask Track(AsyncOperationHandle<GameObject> request, CancellationToken token,
            IProgress<float> progress = null)
        {
            while (!request.IsDone && !token.IsCancellationRequested)
            {
                progress?.Report(request.PercentComplete);
                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }

            // 最终报告100%进度（除非被取消）
            if (!token.IsCancellationRequested)
            {
                progress?.Report(1f);
            }
        }
    }
}