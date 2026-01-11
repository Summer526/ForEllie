using UnityEngine;

public static class ManagerScript
{
    private static GameObject _root;

    private static PoolManager _poolManager;

    private static void CreatePool()
    {
        if (_root == null)
        {
            _root = new GameObject("@Managers");
            Object.DontDestroyOnLoad(_root);
        }
    }

    private static void CreateManager<T>(ref T manager, string name) where T : Component
    {
        if (manager == null)
        {
            CreatePool();

            GameObject obj = new GameObject(name);
            manager = obj.AddComponent<T>();

            Object.DontDestroyOnLoad(obj);

            obj.transform.SetParent(_root.transform);
        }
    }

    public static PoolManager Pool
    {
        get { CreateManager(ref _poolManager, "PoolManager"); return _poolManager; }
    }
}

