using UnityEngine;

namespace MyGame
{
    public class WindowBase
    {
        protected bool m_IsShow = false;
        protected bool m_IsLoaded = false;
        protected GameObject m_GameObject = null;
        protected Transform m_Transform = null; 
        public bool IsShow
        { 
            get
            {
                return m_IsShow;
            }
            set
            {
                m_IsShow = value;
                if (m_IsShow)
                {
                    m_Transform.localScale = Vector3.one;
                    Show();
                }
                else
                {
                    m_Transform.localScale = Vector3.zero;
                    Hide();
                }
            }
        }

        protected virtual void Show()
        {
             //base show ..
        }

        protected virtual void Hide()
        {
            //base hide ..
        }

        public virtual void OnUpdate()
        {
            //base OnUpdate ..
        }

        public virtual void OnAwake()
        {
            //base OnAwake ..
        }

        public virtual void OnDestroy()
        {
            //base OnDestroy ..
        }

        public virtual void OnStart()
        {
            //base OnStart ..
        }

        public virtual bool IsDestroy()
        {
            return true;
        }

        public virtual bool IsTopSortingOrder()
        {
            return false;
        }
    }
}