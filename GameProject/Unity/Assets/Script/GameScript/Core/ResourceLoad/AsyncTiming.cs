using UnityEngine;

namespace MyGame
{
    public class AsyncTiming : IMemoryPool
    {
        private bool _mIsDisposed = false;
        private bool _mIsLoaded = false;  
        private Coroutine _mCoroutine;

        public bool IsLoaded
        {
            get { return _mIsLoaded; }
        }

        public void CancelLoading()
        {
            if (_mIsDisposed || _mIsLoaded)
            {
                return;
            } 
 
            _mIsDisposed = true; 
            _mCoroutine = null;
            System.GC.Collect();
        }

        public void SetCoroutine(Coroutine coroutine)
        {
            _mCoroutine = coroutine;
        }

        public Coroutine GetCoroutine()
        {
            return _mCoroutine;
        } 
       
        public void LoadFinish()
        {
            _mIsLoaded = true; 
        }
    }
}