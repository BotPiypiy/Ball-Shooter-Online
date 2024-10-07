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

    private void Start()
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

    private GameObject SpawnHandler(SpawnMessage msg) => Get(msg.position, msg.rotation);
    private void UnspawnHandler(GameObject spawned) => Return(spawned);

    private void OnDestroy()
    {
        NetworkClient.UnregisterPrefab(_ball);
    }

    private void InitializePool()
    {
        _pool = new Pool<GameObject>(CreateNew, 50);
    }

    private GameObject CreateNew()
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
