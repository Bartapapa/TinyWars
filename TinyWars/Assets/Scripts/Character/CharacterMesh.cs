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

    [Header("CORPSE BEHAVIOR")]
    [SerializeField] private float _yeetForce = 1f;
    [SerializeField] private float _gravityForce = 50f;
    public float GravityForce { get { return _gravityForce; } }

    private bool _facingRight = true;
    public bool FacingRight { get { return _facingRight; } }

    private bool _useGravity = false;

    private void FixedUpdate()
    {
        if (_useGravity)
        {
            _rb.velocity += Vector3.down * _gravityForce * Time.fixedDeltaTime;
        }
    }

    public void Yeet(Vector3 force, Vector3 torque)
    {
        _useGravity = true;
        _rb.isKinematic = false;

        _rb.AddForce(force * _yeetForce, ForceMode.VelocityChange);
        _rb.AddTorque(torque, ForceMode.VelocityChange);
    }

    public bool FlipFacing(bool force = false, bool forceRight = true)
    {
        bool facingRight = false;

        if (force)
        {
            if (forceRight)
            {
                MeshPivot.localRotation = Quaternion.Euler(0, 0, 0);
                _facingRight = true;
            }
            else
            {
                MeshPivot.localRotation = Quaternion.Euler(0, 180, 0);
                _facingRight = false;
            }
        }
        else
        {
            MeshPivot.localRotation = MeshPivot.localRotation * Quaternion.Euler(0, 180, 0);
            _facingRight = !_facingRight;
        }
        
        return facingRight;
    }

    public void ResetState()
    {
        _useGravity = false;

        _rb.isKinematic = true;
        _rb.transform.localRotation = Quaternion.Euler(Vector3.zero);
        _rb.transform.localPosition = Vector3.zero;
    }
}
