using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
    public class ObjectPool : UnitySingleton<ObjectPool>
    {
        //空节点对象池
        private List<GameObject> emptyNodePool = new List<GameObject>();
        private Transform root;

        public override void Awake()
        {
            base.Awake();
            root = transform;
        }

        public static GameObject MallocEmptyNode() 
        {
            if (Instance == null)
            {
                var go = new GameObject("EmptyNode");
                return go;
            }

            GameObject t = Instance.CreateEmptyGameObjectFromPool();
            t.SetActive(true);
            return t;
        }

        public static void FreeEmptyNode(GameObject obj) 
        {
            if (Instance == null)
            {
                return;
            } 
            Instance.DestroyRecycle(obj);
        }
        
        public GameObject CreateEmptyGameObjectFromPool()
        {
            GameObject obj;
            if (emptyNodePool.Count > 0)
            {
                var index = emptyNodePool.Count - 1;
                obj = emptyNodePool[index];
                emptyNodePool.RemoveAt(index);
                return obj;
            } 
            obj = new GameObject("EmptyNode");
            return obj;
        }

        public bool DestroyRecycle(GameObject obj)
        {
            emptyNodePool.Add(obj);
            obj.transform.SetParent(root);
            obj.SetActive(false);
            return true;
        }
    }
}