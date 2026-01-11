using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : MonoBehaviour
{
    private readonly Queue<T> poolQueue = new Queue<T>();
    private readonly T prefab;

    public Transform Root { get; private set; }

    public ObjectPool(T prefab, int count, Transform parent = null)
    {
        this.prefab = prefab;
        Root = new GameObject($"{prefab.name}_pool").transform;
        Object.DontDestroyOnLoad(Root.gameObject);

        if (parent != null)
        {
            Root.SetParent(parent, false);
        }

        for (int i = 0; i < count; i++)
        {
            var obj = GameObject.Instantiate(prefab, Root);
            obj.gameObject.SetActive(false);
            poolQueue.Enqueue(obj);
        }
    }

    public T Dequeue()
    {
        T obj;
        if (poolQueue.Count > 0)
        {
            obj = poolQueue.Dequeue();
        }
        else
        {
            obj = GameObject.Instantiate(prefab, Root);
        }
        obj.gameObject.SetActive(true);
        return obj;
    }

    public void Enqueue(T instance)
    {
        instance.gameObject.SetActive(false);
        poolQueue.Enqueue(instance);
    }
}


