using Unity.Netcode;
using UnityEngine;

public class Wheel : NetworkBehaviour
{
    public WheelCollider wheelCollider;
    public Transform wheelMesh;
    public bool wheelTurn;

    // Sync steer angle with NetworkVariable (infrequent updates)
    private NetworkVariable<float> steerAngle = new NetworkVariable<float>(
        writePerm: NetworkVariableWritePermission.Owner
    );

    // Local-only variable for smooth wheel rotation
    private float visualWheelRpm = 0f;

    // Update the visual wheel RPM across clients
    [ClientRpc]
    private void UpdateWheelRpmClientRpc(float rpm)
    {
        visualWheelRpm = rpm;
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        // Send the current steering angle to all clients
        steerAngle.Value = wheelCollider.steerAngle;

        // Send the current RPM to all clients
        UpdateWheelRpmClientRpc(wheelCollider.rpm);
    }

    private void Update()
    {
        if (!IsClient) return; // Only clients need to update visuals

        // Update steering visual if applicable
        if (wheelTurn)
        {
            Vector3 angles = wheelMesh.localEulerAngles;
            angles.y = steerAngle.Value;
            wheelMesh.localEulerAngles = angles;
        }

        // Rotate the wheel mesh based on the synced RPM
        float deltaRotation = visualWheelRpm / 60f * 360f * Time.deltaTime;
        wheelMesh.Rotate(deltaRotation, 0f, 0f);
    }
}