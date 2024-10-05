using Mirror;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [SerializeField]
    [Min(1f)]
    private float _movementSpeed = 10f;
    [SerializeField]
    [Min(1f)]
    private float _mouseSensevity = 100f;

    [Space]
    [SerializeField]
    private Transform _camera;

    [Space]
    [Header("Camera Constraints")]
    [SerializeField]
    private float _minX = -60;
    [SerializeField]
    private float _maxX = 20;
    [SerializeField]
    private float _minY = -30;
    [SerializeField]
    private float _maxY = 30;

    [Header("Components")]
    [SerializeField]
    private Rigidbody _rigidbody;

    [Header("Meshes")]
    [SerializeField]
    private List<MeshRenderer> _meshes;
    [SerializeField]
    private List<ColorMaterial> _colorMaterials;

    [Header("Shooting")]
    [SerializeField]
    private Transform _shootPoint;
    [SerializeField]
    private float _minPower = 5f;
    [SerializeField]
    private float _maxPower = 50f;

    private float _currentPower = 0f;

    private float _currentXRotation = 0f;
    private float _currentYRotation = 0f;

    [SyncVar(hook = nameof(SetName))]
    private string _name;

    void Start()
    {
        Application.targetFrameRate = 60;
        _name = gameObject.name;

        ApplyName(_name);
    }

    void Update()
    {
        if (this.isLocalPlayer)
        {
            MovementUpdate(Time.deltaTime);
            CameraMovementUpdate(Time.deltaTime);
            TryShoot(Time.deltaTime);
        }
    }

    private void SetName(string _, string newName)
    {
        ApplyName(newName);
    }

    private void ApplyName(string name)
    {
        gameObject.name = name;
        Material material = _colorMaterials.Find(material => material.Color == transform.name[0]).Material;
        foreach (var mesh in _meshes)
        {
            mesh.material = material;
        }

        if (!this.isLocalPlayer)
        {
            _camera.gameObject.GetComponent<Camera>().enabled = false;
        }
    }

    private void MovementUpdate(float deltaTime)
    {
        float x = Input.GetAxis("Horizontal") * _movementSpeed * deltaTime;
        _rigidbody.position += transform.right * x;
    }


    private void CameraMovementUpdate(float deltaTime)
    {
        float x = Input.GetAxis("Mouse X") * _mouseSensevity * deltaTime;
        float y = Input.GetAxis("Mouse Y") * _mouseSensevity * deltaTime;

        _currentXRotation -= y;
        _currentXRotation = Mathf.Clamp(_currentXRotation, _minX, _maxX);

        _currentYRotation += x;
        _currentYRotation = Mathf.Clamp(_currentYRotation, _minY, _maxY);

        _camera.localRotation = Quaternion.Euler(_currentXRotation, _currentYRotation, 0f);
    }

    private void TryShoot(float deltaTime)
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            _currentPower += deltaTime * 100f;
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            _currentPower = Mathf.Clamp(_currentPower, _minPower, _maxPower);
            CmdShoot(_shootPoint.forward * _currentPower, gameObject.name);
            _currentPower = 0f;
        }
    }

    [Command]
    private void CmdShoot(Vector3 power, string name)
    {
        GameObject ball = BallPool.Instance.Get(_shootPoint.position, _shootPoint.rotation);
        ball.name = name;
        NetworkServer.Spawn(ball);
        ball.GetComponent<Rigidbody>().AddForce(power, ForceMode.Impulse);
    }
}

[Serializable]
public struct ColorMaterial
{
    [SerializeField]
    public char Color;
    [SerializeField]
    public Material Material;
}
