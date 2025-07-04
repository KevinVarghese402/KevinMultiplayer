using UnityEngine;
using Unity.Netcode;

public class BoostableCar : NetworkBehaviour
{
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Boost(float boostForce)
    {
        if (!IsOwner) return; // Only apply boost on the local player
        Vector3 boostDirection = transform.forward;
        rb.AddForce(boostDirection * boostForce, ForceMode.VelocityChange);
    }
}