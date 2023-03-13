using UnityEngine;

public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static object m_SyncObj = new object ();
    private static bool m_IsQuit = false;

    private static volatile T m_Instance;
    public static T Instance {
        get {
            if ( m_IsQuit )
            {
                return null;
            }

            if ( m_Instance == null )
            {
                m_Instance = FindObjectOfType<T> () as T;

                if ( 1 < FindObjectsOfType<T> ().Length )
                {
                    // 複数Find
                    return m_Instance;
                }

                // 生成
                if ( m_Instance == null )
                {
                    lock ( m_SyncObj )
                    { 
                        GameObject _Singleton = new GameObject ();
                        _Singleton.name = typeof ( T ).ToString () + " (singleton)";
                        m_Instance = _Singleton.AddComponent<T> ();
                        DontDestroyOnLoad ( _Singleton );
                    }
                }

            }
            return m_Instance;
        }
        private set { m_Instance = value; }
    }

    #region UNITY EVENT
    void OnDestroy ()
    {
        Instance = null; // Setter
    }

    void OnApplicationQuit ()
    {
        m_IsQuit = true;
    }
    #endregion

    protected SingletonMonoBehaviour ()
    {
    }
}
