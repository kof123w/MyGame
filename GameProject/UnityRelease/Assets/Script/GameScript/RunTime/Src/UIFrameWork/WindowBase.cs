using UnityEngine;

namespace MyGame
{
    public abstract class WindowBase : IUIWindow
    {
        protected bool m_IsShow = false;
        protected bool m_IsLoaded = false;
        protected GameObject m_GameObject = null;
        
        void IUIWindow.Show()
        {
            throw new System.NotImplementedException();
        }

        void IUIWindow.Hide()
        {
            throw new System.NotImplementedException();
        }

        void IUIWindow.OnUpdate(float deltaTime)
        {
            throw new System.NotImplementedException();
        }

        void IUIWindow.OnAwake()
        {
            throw new System.NotImplementedException();
        }

        void IUIWindow.OnDestroy()
        {
            throw new System.NotImplementedException();
        }

        void IUIWindow.OnStart()
        {
            throw new System.NotImplementedException();
        }
    }
}