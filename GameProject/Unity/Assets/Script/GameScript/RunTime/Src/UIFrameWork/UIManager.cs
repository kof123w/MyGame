using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MyGame
{
    public class UIManager : Singleton<UIManager>
    { 
        private int m_AddWindowSortNormal = 100;
        private List<UIWindow> m_WindowList = new List<UIWindow>();
        
        private Transform _mRoot;
        public static void InitUIManager()
        {
            if (Instance is not null)
            {
                Instance._mRoot = GameObject.Find("UIRoot").transform;
            }
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
                T t = hasWindow ? (T)list[index] : new T(); 
                if (t != null)
                {
                    int maxSort = t.IsTopSortingOrder() ? short.MaxValue : Instance.GetMaxSort(); 
                    t.SetWindowSortingOrder(ref maxSort);
                    t.IsShow = true;
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