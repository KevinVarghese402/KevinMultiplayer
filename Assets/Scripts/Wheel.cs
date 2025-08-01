using System;
using Unity.Netcode;
using UnityEngine;

public class Wheel : NetworkBehaviour
{
    public WheelCollider wheelCollider;
    public Transform wheelMesh;
    public bool wheelTurn;

    // New: sync rpm from server to clients
    private NetworkVariable<float> syncedRPM = new NetworkVariable<float>(
        0f, 
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Server);
    
    private NetworkVariable<float> syncedSteerAngle = new NetworkVariable<float>(
        0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private void Update()
    {
        if (IsServer)
        {
            // Only the server sets this value
            syncedRPM.Value = wheelCollider.rpm;
            
            if (wheelTurn)
                syncedSteerAngle.Value = wheelCollider.steerAngle;
        }
        
        RotateWheelMesh(syncedRPM.Value);
        
        if (wheelTurn)
        {
            Vector3 currentRotation = wheelMesh.localEulerAngles;
            wheelMesh.localEulerAngles = new Vector3(
                currentRotation.x,
                syncedSteerAngle.Value - currentRotation.z,
                currentRotation.z
            );
        }
        
    }

    private void RotateWheelMesh(float rpm)
    {
        float rotationSpeed = rpm / 60f * 360f * Time.deltaTime;
        wheelMesh.Rotate(rotationSpeed, 0, 0);
    }
}