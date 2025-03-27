using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
    public class UIManager : Singleton<UIManager>
    { 
        private int m_AddWindowSortNormal = 100;
        private List<UIWindow> m_WindowList = new List<UIWindow>();
        
        private Transform _mRoot;
        public void InitUIManager()
        {
            _mRoot = GameObject.Find("UIRoot").transform; 
        }
       
        public static Transform GetUIRoot()
        {
            if (Instance is null)
            {
                return null;
            }

            return Instance._mRoot;
        }

        public static void ShowWindow<T>() where T : UIWindow, new()
        {
            if (Instance is not null)
            { 
                var list = Instance.m_WindowList;
                int index = list.FindIndex(w => w.GetType() == typeof(T)); 
                var hasWindow = index!=-1;
                T t = hasWindow ? (T)list[index] : Pool.Malloc<T>(); 
                if (t != null)
                {
                    int maxSort = t.IsTopSortingOrder() ? short.MaxValue : Instance.GetMaxSort(); 
                    maxSort += Instance.m_AddWindowSortNormal;
                    t.SetWindowSortingOrder(ref maxSort);
                    t.IsShow = true;
                    t.SetWindowType(typeof(T));
                    t.LoadResource();
                }
            }
        }

        public static T GetWindow<T>() where T : UIWindow, new()
        {
            if (Instance is not null)
            { 
                var list = Instance.m_WindowList;
                int index = list.FindIndex(w => w.GetType() == typeof(T)); 
                
                var hasWindow = index!=-1;
                if (hasWindow)
                {
                    return list[index] as T;  
                } 
            }
            
            return null;
        } 
        public static void HideWindow<T>()where T : UIWindow, new()
        {
            if (Instance is not null)
            { 
                var list = Instance.m_WindowList;
                int index = list.FindIndex(w => w.GetType() == typeof(T)); 
                
                var hasWindow = index!=-1;
                if (hasWindow)
                {
                   T t = list[index] as T;  
                   t.IsShow = false;
                } 
            }
        }

        public static void Close<T>() where T : UIWindow, new()
        {
            if (Instance is not null)
            { 
                var list = Instance.m_WindowList;
                int index = list.FindIndex(w => w.GetType() == typeof(T)); 
                
                var hasWindow = index!=-1;
                if (hasWindow)
                {
                    T t = list[index] as T;
                    if (t != null)
                    {
                        Pool.Free(t);
                    } 
                } 
            }
        }

        public void Update()
        {
            for (int i = 0; i < m_WindowList.Count; i++) {
                var window = m_WindowList[i];
                if (window.IsDestroy() || !window.IsLoaded())
                {
                    continue;
                }
                window.OnUpdate();
            }
        }

        private int GetMaxSort()
        {
            int max = 0;
            for (int i = 0; i < m_WindowList.Count; i++)
            {
                var uiWindow = m_WindowList[i];
                uiWindow.GetWindowSortingOrder(out var tmp);
                if (tmp > max)
                {
                    max = tmp;
                } 
            } 
            return max;
        }
    }
}