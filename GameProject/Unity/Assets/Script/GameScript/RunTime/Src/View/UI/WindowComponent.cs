using UnityEditor.PackageManager.UI;
using UnityEngine;

namespace MyGame
{
    public class WindowComponent : ComponentData
    {
        protected GameObject m_go;

        public GameObject GetGameObject()
        {
            return m_go;
        }
    }
}