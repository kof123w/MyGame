using UnityEngine;

namespace MyGame
{
    public class WindowBase:IMemoryPool
    {
        protected bool MIsShow = false;
        protected bool MIsLoaded = false;
        protected GameObject MGameObject = null;
        protected Transform MTransform = null;
        protected bool MIsShowed = false;
        protected bool MIsHidded = false; 
        protected bool MIsDestroyed = false;

        public bool IsShow
        { 
            get
            {
                return MIsShow;
            }
            set
            {
                MIsShow = value;
                if (MIsShow)
                {
                    if (MGameObject != null)
                    {
                        MGameObject.SetActive(true);
                    }

                    if (MIsLoaded)
                    {
                        Show();
                    } 
                }
                else
                {
                    if (MGameObject != null)
                    {
                        MGameObject.SetActive(false);
                    }

                    if (MIsLoaded)
                    {
                        Hide();
                    } 
                }
            }
        }

        public bool IsLoaded()
        {
            return MIsLoaded;
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
            return MIsDestroyed;
        }

        public virtual bool IsTopSortingOrder()
        {
            return false;
        }
    }
}