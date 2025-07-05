using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Unity.Netcode;

public class BoostableCar : NetworkBehaviour
{
    public float boostForce = 10f;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void RequestBoost()
    {
        if (rb != null)
        {
            Vector3 boostDirection = transform.forward;
            rb.AddForce(boostDirection * boostForce, ForceMode.VelocityChange);
        }
    }

}
