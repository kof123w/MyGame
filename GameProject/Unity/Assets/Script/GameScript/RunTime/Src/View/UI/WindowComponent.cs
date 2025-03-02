using UnityEditor.PackageManager.UI;
using UnityEngine;

namespace MyGame
{
    public class WindowComponent : ComponentData
    {
        protected GameObject m_obj;
        protected bool m_isLoaded = false;
        public GameObject GetGameObject()
        {
            return m_obj;
        }

        public void SetGameObject(GameObject go)
        {
            m_obj = go;
        }

        public bool IsLoaded()
        {
            return m_isLoaded;
        }
    }
}