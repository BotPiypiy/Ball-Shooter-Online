using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
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
    List<MeshRenderer> _meshes;
    [SerializeField]
    List<ColorMaterial> _colorMaterials;


    private float _currentXRotation = 0f;
    private float _currentYRotation = 0f;

    void Start()
    {
        Material material = _colorMaterials.Find(material => material.Color == transform.name[0]).Material;
        foreach (var mesh in _meshes)
        {
            mesh.material = material;
        }
    }


    void Update()
    {
        MovementUpdate();
        CameraMovementUpdate();
    }

    private void MovementUpdate()
    {
        float x = Input.GetAxis("Horizontal") * _movementSpeed * Time.deltaTime;
        _rigidbody.position += transform.right * x;
    }

    private void CameraMovementUpdate()
    {
        float x = Input.GetAxis("Mouse X") * _mouseSensevity * Time.deltaTime;
        float y = Input.GetAxis("Mouse Y") * _mouseSensevity * Time.deltaTime;

        _currentXRotation -= y;
        _currentXRotation = Mathf.Clamp(_currentXRotation, _minX, _maxX);

        _currentYRotation += x;
        _currentYRotation = Mathf.Clamp(_currentYRotation, _minY, _maxY);

        _camera.localRotation = Quaternion.Euler(_currentXRotation, _currentYRotation, 0f);
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
