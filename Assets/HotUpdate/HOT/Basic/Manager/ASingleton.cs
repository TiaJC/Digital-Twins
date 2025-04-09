using UnityEngine;


public abstract class ASingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    private static object _lock = new object();
    public static T RawInstance
    {
        get { return _instance; }
    }

    public static T Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    var objs = FindObjectsOfType(typeof(T));
                    if (objs.Length > 0)
                    {
                        if (objs.Length != 1)
                        {
                            Debug.LogError("[Singleton] Something went really wrong - there should never be more than 1 singleton:" + typeof(T));

                            foreach (var item in objs)
                            {
                                Debug.LogError(item.name);
                            }
                            return null;
                        }
                        else
                            _instance = objs[0] as T;
                    }
                    else
                    {
                        GameObject singleton = new GameObject();
                        _instance = singleton.AddComponent<T>();
                        singleton.name = typeof(T).ToString();
                    }
                }
                return _instance;
            }
        }
    }

    protected virtual void Awake()
    {
#if UNITY_EDITOR
        if (FindObjectsOfType(typeof(T)).Length > 1)
        {
            Debug.LogError("[Singleton] Something went really wrong - there should never be more than 1 singleton" + typeof(T));
            return;
        }
#endif
        _instance = this as T;

        if (_instance.transform.parent == null)
        {
            DontDestroyOnLoad(_instance.gameObject);
        }
    }


}

