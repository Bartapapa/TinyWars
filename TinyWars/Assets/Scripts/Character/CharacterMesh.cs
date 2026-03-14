using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMesh : MonoBehaviour
{
    [Header("OBJECT REFERENCES")]
    [SerializeField] private Transform _meshPivot;
    public Transform MeshPivot { get { return _meshPivot; } }
    [SerializeField] private Rigidbody _rb;
    public Rigidbody Rigidbody { get { return _rb; } }

    public void Yeet(Vector3 force, Vector3 torque)
    {
        _rb.useGravity = true;

        _rb.AddForce(force, ForceMode.VelocityChange);
        _rb.AddTorque(torque, ForceMode.VelocityChange);
    }
}
