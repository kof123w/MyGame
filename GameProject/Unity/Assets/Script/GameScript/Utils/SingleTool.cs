 
using UnityEngine;

namespace MyGame
{
    /// <summary>
    /// 普通单例工具
    /// </summary>
    public abstract class Singleton<T> where T :class,new (){
        private static T instance;
        private static System.Object mutex = new System.Object();
      
        public static T Instance {
            get {
                if (instance == null) {
                    lock (mutex) {    //确保单列初始化是线程安全
                        if (instance == null) { 
                            instance = new T();
                        }
                    }
                }
  
                return instance;
            }
        }

        public static void ReleaseInstance()
        {
            if (instance != null)
            {
                lock (mutex)
                {
                    // ReSharper disable once RedundantCheckBeforeAssignment
                    if (instance != null)
                    {
                        instance = null;
                    }
                }
            }
        }
    }
  
    /// <summary>
    /// unity的单列工具
    /// </summary> 
    public class UnitySingleton<T> : MonoBehaviour where T : Component {
        private static T instance;
        public static T Instance
        {
            get {
                if (instance==null) {
#pragma warning disable CS0618 // Type or member is obsolete
                    instance =  FindObjectOfType(typeof(T)) as T;
#pragma warning restore CS0618 // Type or member is obsolete
                    if (instance == null) {
                        GameObject go = new GameObject();
                        instance = (T)go.AddComponent(typeof(T));
                        go.hideFlags = HideFlags.DontSave;
                        go.name = typeof(T).Name;
                    }
                }
  
                return (T)instance;
            }
        }
  
        public virtual void Awake() { 
            DontDestroyOnLoad(this.gameObject);
            if (instance == null)
            {
                instance = this as T;
            }
            else {
                Destroy(instance);
            }
        }
    }
}