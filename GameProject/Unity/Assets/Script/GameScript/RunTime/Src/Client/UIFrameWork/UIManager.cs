using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MyGame
{
    public class UIManager : Singleton<UIManager>
    { 
        private int addWindowSortNormal = 100;
        private List<UIWindow> windowList = new List<UIWindow>();
        private readonly List<UIController> uIControllerList = new List<UIController>();
        private readonly List<UIModel> uiModelList = new List<UIModel>();
        private Transform root;

        public int GetAddWindowSortNormal()
        {
            return addWindowSortNormal;
        }

        public static Transform GetRoot()
        {
            return Instance.root;
        }

        public void Init()
        {
            DLogger.Log("==============>InitUIManager");
            root = GameObject.Find("UIRoot").transform; 
            uIControllerList.Clear(); 
        }

        public List<UIWindow> GetUIWindowList()
        {
            return windowList;
        }

        public void Tick()
        {
            foreach (var window in windowList.Where(window => !window.IsDestroy() && window.IsLoaded() && window.IsShow))
            {
                window.OnUpdate();
            }
        }

        public Transform GetUIRoot()
        {
            return root;
        } 

        public int GetMaxSort()
        {
            int max = 0;
            for (int i = 0; i < windowList.Count; i++)
            {
                var uiWindow = windowList[i];
                uiWindow.GetWindowSortingOrder(out var tmp);
                if (tmp > max)
                {
                    max = tmp;
                } 
            } 
            return max;
        }

        public void AddController(UIController controller,UIModel uiModel)
        {
            controller.InitController();
            uIControllerList.Add(controller); 
        }

        public void AddModel(UIModel uiModel)
        {
            uiModelList.Add(uiModel);
        } 
        
        public static async UniTask Show<T>() where T : UIWindow, new()
        {
            var list = Instance.GetUIWindowList();
            int index = list.FindIndex(w => w.GetType() == typeof(T)); 
            var hasWindow = index!=-1;
            T t = hasWindow ? (T)list[index] : Pool.Malloc<T>(); 
            if (t != null)
            {
                var maxSort = t.IsTopSortingOrder() ? short.MaxValue : Instance.GetMaxSort(); 
                maxSort += Instance.GetAddWindowSortNormal();
                t.SetWindowSortingOrder(ref maxSort);
                t.IsShow = true;
                t.SetWindowType(typeof(T));
                var obj = await t.LoadResource();
                t.OnLoadComplete((GameObject)Object.Instantiate(obj));
                list.Add(t);
            }
        }

        public static async UniTask<T> GetWindow<T>() where T : UIWindow, new()
        {
            var list = Instance.GetUIWindowList();
            int index = list.FindIndex(w => w.GetType() == typeof(T)); 
                
            var hasWindow = index!=-1;
            if (hasWindow)
            {
                if (list[index] is T t)
                {
                    await UniTask.WaitUntil(t.IsLoaded);
                    return t;
                }
            }

            return null;
        } 
        public static void Hide<T>()where T : UIWindow, new()
        {
            var list = Instance.GetUIWindowList();
            int index = list.FindIndex(w => w.GetType() == typeof(T)); 
                
            var hasWindow = index!=-1;
            if (hasWindow)
            {
                if (list[index] is T t) t.IsShow = false;
            }
        }

        public static void Close<T>() where T : UIWindow, new()
        {
            var list = Instance.GetUIWindowList();
            int index = list.FindIndex(w => w.GetType() == typeof(T)); 
                
            var hasWindow = index!=-1;
            if (hasWindow)
            {
                if (list[index] is T t)
                {
                    if (t.OnDestroyIsDestroy())
                    {
                        t.Destroy();
                        Pool.Free(t);
                        list.RemoveAt(index);
                    }
                    else
                    {
                        t.OnDestroy();
                        t.IsShow = false;
                    }
                } 
            } 
        } 
    }
}