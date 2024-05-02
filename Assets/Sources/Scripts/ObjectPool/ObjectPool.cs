using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectPool<T> where T : MonoBehaviour
{
    private List<T> _pool;

    public List<T> Prefabs { get; private set; }
    public Transform Container { get; private set; }
    public int Count => _pool.Count;
    public bool IsEnded { get; private set; } = true;

    public ObjectPool(List<T> prefab)
    {
        Prefabs = prefab;
        Container = null;

        CreatePool();
    }

    public ObjectPool(List<T> prefab, Transform container)
    {
        Prefabs = prefab;
        Container = container;

        CreatePool();
    }

    private void CreatePool()
    {
        _pool = new List<T>();

        foreach (T item in Prefabs)
        {
            CreateObject(item);
        }
    }

    private void ShufflePool()
    {
        int number = _pool.Count;

        while (number > 1)
        {
            number--;
            int index = Random.Range(0, number + 1);
            T value = _pool[index];

            _pool[index] = _pool[number];
            _pool[number] = value;
        }
    }

    private T CreateObject(T obj, bool isActiveByDefault = false)
    {
        obj.gameObject.SetActive(isActiveByDefault);
        _pool.Add(obj);

        return obj;
    }

    public bool HasFreeElement(out T element)
    {
        foreach (var item in _pool)
        {
            if (!item.gameObject.activeInHierarchy)
            {
                element = item;
                item.gameObject.SetActive(true);
                return true;
            }
        }

        element = null;

        return false;
    }

    public T GetFreeElement()
    {
        ShufflePool();

        if (HasFreeElement(out var element))
            return element;

        throw new System.Exception($"There is no free elements in pool of type {typeof(T)}");
    }
}
