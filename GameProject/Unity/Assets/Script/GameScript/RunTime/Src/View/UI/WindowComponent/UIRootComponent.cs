using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
    public class UIRootComponent : WindowComponent
    {
        public Transform UIRoot { get; set; }
        
        private Dictionary<long,int> m_UILowWindowCanvas = new Dictionary<long, int>();
        private Dictionary<long,int> m_UIMidWindowCanvas = new Dictionary<long, int>();
        private Dictionary<long,int> m_UIHighWindowCanvas = new Dictionary<long, int>();
        
        private int LowAddSorting = 50;  //层级自动加50
        private int MidAddSorting = 500;  //层级自动加500
        private int HighAddSorting = 1000;  //层级自动加1000
        
        private string m_EventOpenWindow = "UIRootComponent_UIEventOpenWindow";
        private string m_EventCloseWindow = "UIRootComponent_UIEventCloseWindow";
        
        public long EventOpenWindowEventID => m_EventOpenWindow.StringToHash();
        public long EventCloseWindowEventID => m_EventCloseWindow.StringToHash();                                        
    }
}