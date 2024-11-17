using UnityEngine;
using System.Collections.Generic;

public class ObjectPool<T> where T : MonoBehaviour
{
    #region Fields

    private T _prefab;
    private readonly Queue<T> _pool = new Queue<T>();
    private Transform _parent;

    #endregion

    #region Public Methods

    public ObjectPool(T prefab, int initialSize, Transform parent = null)
    {
        _prefab = prefab;
        _parent = parent;

        for (int i = 0; i < initialSize; i++)
        {
            T obj = GameObject.Instantiate(_prefab, _parent);
            obj.gameObject.SetActive(false);
            _pool.Enqueue(obj);
        }
    }

    public T GetObject()
    {
        if (_pool.Count > 0)
        {
            T obj = _pool.Dequeue();
            obj.transform.parent = null;
            obj.gameObject.SetActive(true);
            return obj;
        }

        T newObj = GameObject.Instantiate(_prefab, _parent);
        return newObj;
    }

    public void ReturnObject(T obj)
    {
        obj.transform.parent = _parent;
        obj.gameObject.SetActive(false);
        _pool.Enqueue(obj);
    }

    #endregion
}