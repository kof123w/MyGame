using System;
using System.Collections.Generic;
using System.Threading;
using AssetsLoad;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MyGame
{ 
    public class UIWindow : WindowBase
    {
        protected int MSort;
        protected Canvas MCanvas;
        protected Type MWindowType; 
        protected CancellationTokenSource cancellationTokenSource;
        
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

        public async UniTask<Object> LoadResource()
        {
            if (cancellationTokenSource == null)
            {
                cancellationTokenSource = new CancellationTokenSource();
            }
            else
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
                cancellationTokenSource = new CancellationTokenSource();
            }

            return await ResourcerDecorator.Instance.LoadUIResourceAsync(MWindowType.Name,cancellationTokenSource.Token); 
        }

        public void Destroy()
        {
            OnDestroy(); 

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
            MTransform.SetParent(UIManager.GetRoot(),false);
            OnAwake();
            OnStart(); 
        } 
    }
}