using UnityEngine;

namespace MyGame
{
    public class AsyncTiming : IMemoryPool
    {
        private bool mIsDisposed = false;
        private bool mIsLoaded = false;  
        private Coroutine mCoroutine;

        public bool IsLoaded
        {
            get { return mIsLoaded; }
        }

        public void CancelLoading()
        {
            if (mIsDisposed || mIsLoaded)
            {
                return;
            } 
 
            mIsDisposed = true; 
            mCoroutine = null;
            System.GC.Collect();
        }

        public void SetCoroutine(Coroutine coroutine)
        {
            mCoroutine = coroutine;
        }

        public Coroutine GetCoroutine()
        {
            return mCoroutine;
        } 
       
        public void LoadFinish()
        {
            mIsLoaded = true; 
        }
    }
}