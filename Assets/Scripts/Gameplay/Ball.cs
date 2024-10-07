using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
 
    public PlayerController Owner { get => _owner; set => _owner = value; }
    private PlayerController _owner;

    [SerializeField]
    private float _liveTime;

    private Rigidbody _rb;

    private Coroutine _liveCo;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        _rb.velocity = Vector3.zero;
        _owner = null;
        _liveCo = StartCoroutine(LiveCoroutine());
    }

    private IEnumerator LiveCoroutine()
    {
        yield return new WaitForSeconds(_liveTime);
        _liveCo = null;
        DestroySelf();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.transform.parent.TryGetComponent(out PlayerController otherPlayer) && _owner != null)
        {
            _owner.Hit();
            otherPlayer.Hitted();
            DestroySelf();
        }
    }

    [Server]
    private void DestroySelf()
    {
        if(_liveCo != null)
        {
            StopCoroutine(_liveCo);
            _liveCo = null;
        }
        NetworkServer.UnSpawn(gameObject);
        BallPool.Instance.Return(gameObject);
    }
}
