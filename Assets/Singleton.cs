using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static readonly object _lock = new object();

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = FindObjectOfType<T>();
                        if (_instance == null)
                        {
                            GameObject singletonObject = new GameObject();
                            _instance = singletonObject.AddComponent<T>();
                            singletonObject.name = typeof(T).ToString() + " (Singleton)";

                            Debug.Log($"[Singleton] An instance of {typeof(T)} is needed in the scene, so '{singletonObject}' was created.");
                        }
                        else
                        {
                            Debug.Log($"[Singleton] Using instance already present in the scene: {_instance.gameObject.name}");
                        }
                    }
                }
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
        }
        else if (_instance != this)
        {
            Debug.LogWarning($"[Singleton] Another instance of {typeof(T)} already exists. Destroying the new one: {this.gameObject.name}");
            Destroy(this.gameObject);
        }
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
}