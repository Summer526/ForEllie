using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    private static Dictionary<string, object> pools = new Dictionary<string, object>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CreatePool<T>(T prefab, int generateCount, Transform parent = null) where T : MonoBehaviour
    {
        if (prefab == null) return;

        string key = prefab.GetInstanceID().ToString();
        if (pools.ContainsKey(key)) return;

        pools.Add(key, new ObjectPool<T>(prefab, generateCount, parent));
    }

    public T GetFromPool<T>(T prefab) where T : MonoBehaviour
    {
        if (prefab == null) return null;

        string key = prefab.GetInstanceID().ToString();
        if (!pools.TryGetValue(key, out var box)) return null;

        return (box as ObjectPool<T>)?.Dequeue();
    }

    public void ReturnPool<T>(T instance) where T : MonoBehaviour
    {
        if (instance == null) return;

        string key = instance.GetInstanceID().ToString();
        if (!pools.TryGetValue(key, out var box))
        {
            Destroy(instance.gameObject);
            return;
        }

        (box as ObjectPool<T>)?.Enqueue(instance);
    }
}

