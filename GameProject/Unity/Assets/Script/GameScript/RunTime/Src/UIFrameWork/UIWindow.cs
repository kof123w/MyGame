using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MyGame
{ 
    public class UIWindow : WindowBase
    {
        protected int MSort;
        protected Canvas MCanvas;
        protected Type MWindowType;
        private AsyncTiming MAsyncTiming;
        
        public void SetWindowSortingOrder(ref int sort)
        {
            MSort = sort;
        } 

        public void SetWindowType(Type windowType)
        {
            MWindowType = windowType;
        }

        public void GetWindowSortingOrder(out int sort)
        {
            sort = MSort;
        }

        public void LoadResource()
        {
            MAsyncTiming = ResourceLoader.Instance.LoadUIResource(UIManager.GetUIRoot(),OnLoadComplete,MWindowType.Name);
        }

        public void Destroy()
        {
            OnDestroy();
            if (MAsyncTiming != null)
            {
                if (!MAsyncTiming.IsLoaded)
                {
                    ResourceLoader.Instance.CancelLoading(MAsyncTiming);
                }
            }

            MTransform = null;
            Object.DestroyImmediate(MGameObject);
            MIsDestroyed = true;
        }

        public void OnLoadComplete(GameObject go)
        {
            MCanvas = go.GetComponent<Canvas>();
            if (MCanvas != null)
            {
                MCanvas.sortingOrder = MSort;
            }
            MIsLoaded = true;
            if (IsShow)
            {
                Show();
            }else
            {
                Hide();
            }

            MGameObject = go;
            MTransform = go.transform; 
            OnAwake();
            OnStart();
        }
    }
}