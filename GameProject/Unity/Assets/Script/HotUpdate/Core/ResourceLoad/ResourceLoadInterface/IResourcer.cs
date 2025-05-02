using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace AssetsLoad
{
    public interface IResourcer
    {
        public void UnloadUnusedAssets();
        UniTask<Object> LoadAsync(string resName,CancellationToken token,IProgress<float> progress = null); 
        UniTask Track(AsyncOperationHandle<GameObject> request,CancellationToken token,IProgress<float> progress = null);
    } 
}