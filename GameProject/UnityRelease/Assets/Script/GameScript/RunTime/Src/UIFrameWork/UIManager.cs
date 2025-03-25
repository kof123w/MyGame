using System;
using System.Collections;
using System.Collections.Generic;

namespace MyGame
{
    public class UIManager : Singleton<UIManager>
    {
        private Dictionary<Type, int> m_WindowTypeSort = new Dictionary<Type, int>();
        private int m_AddWindowSortNormal = 50;
        private List<UIWindow> m_WindowList = new List<UIWindow>(); 

        public static void ShowWindow<T>() where T : WindowBase, new()
        {
            if (Instance is { } uiManager)
            {
                //
            }
        }


        private int GetMaxSort()
        {
            int max = 0;
            foreach (var pair in m_WindowTypeSort)
            {
                if (max < pair.Value)
                {
                    max = pair.Value;
                }
            }

            return max;
        }
    }
}