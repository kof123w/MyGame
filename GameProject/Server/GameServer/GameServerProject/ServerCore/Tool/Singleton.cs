namespace MyServer;
public abstract class Singleton<T> where T : class, new()
{
    private static T instance;
    private static Object mutex = new Object();

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                lock (mutex)
                {
                    //确保单列初始化是线程安全
                    if (instance == null)
                    {
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