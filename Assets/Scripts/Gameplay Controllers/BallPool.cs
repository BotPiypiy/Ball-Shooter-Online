using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPool : MonoBehaviour
{
    // singleton for easier access from other scripts
    public static BallPool Instance;

    [SerializeField]
    private GameObject _ball;

    private Pool<GameObject> _pool;

    void Start()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        InitializePool();
        NetworkClient.RegisterPrefab(_ball, SpawnHandler, UnspawnHandler);
    }

    GameObject SpawnHandler(SpawnMessage msg) => Get(msg.position, msg.rotation);
    void UnspawnHandler(GameObject spawned) => Return(spawned);

    void OnDestroy()
    {
        NetworkClient.UnregisterPrefab(_ball);
    }

    void InitializePool()
    {
        _pool = new Pool<GameObject>(CreateNew, 50);
    }

    GameObject CreateNew()
    {
        GameObject next = Instantiate(_ball, transform);
        next.SetActive(false);
        return next;
    }

    public GameObject Get(Vector3 position, Quaternion rotation)
    {
        GameObject next = _pool.Get();

        next.transform.position = position;
        next.transform.rotation = rotation;
        next.SetActive(true);
        return next;
    }

    public void Return(GameObject spawned)
    {
        spawned.SetActive(false);
        _pool.Return(spawned);
    }
}
