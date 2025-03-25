using UnityEngine;

namespace MyGame
{
    public class AsyncTiming : IMemoryPool
    {
        private bool _mIsDisposed = false;
        private bool _mIsLoaded = false; 
        private AssetBundleCreateRequest _mAssetBundleCreateRequest; 
        private Coroutine _mCoroutine;
        public void CancelLoading()
        {
            if (_mIsDisposed || _mIsLoaded)
            {
                return;
            } 

            if (_mAssetBundleCreateRequest != null && _mAssetBundleCreateRequest.assetBundle != null)
            {
                _mAssetBundleCreateRequest.assetBundle.Unload(true);
                _mAssetBundleCreateRequest = null;
                System.GC.Collect();
            } 
            _mIsDisposed = true;
            _mAssetBundleCreateRequest = null;
            _mCoroutine = null;
        }

        public void SetCoroutine(Coroutine coroutine)
        {
            _mCoroutine = coroutine;
        }

        public Coroutine GetCoroutine()
        {
            return _mCoroutine;
        }

        public void SetAssetBundleCreateRequest(AssetBundleCreateRequest abr)
        {
            _mAssetBundleCreateRequest = abr;
        }

        public void LoadFinish()
        {
            _mIsLoaded = true;
            _mAssetBundleCreateRequest = null;
        }
    }
}