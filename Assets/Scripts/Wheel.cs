using Unity.Netcode;
using UnityEngine;

public class Wheel : NetworkBehaviour
{
    public WheelCollider wheelCollider;
    public Transform wheelMesh;
    public bool wheelTurn;

    // Synced data
    private NetworkVariable<float> steerAngle = new NetworkVariable<float>(
        writePerm: NetworkVariableWritePermission.Owner
    );

    private NetworkVariable<float> wheelRpm = new NetworkVariable<float>(
        writePerm: NetworkVariableWritePermission.Owner
    );

    private void FixedUpdate()
    {
        // Only the owning client should update these
        if (!IsOwner) return;

        steerAngle.Value = wheelCollider.steerAngle;
        wheelRpm.Value = wheelCollider.rpm;
    }

    private void Update()
    {
        // All clients use the synced data to rotate wheels
        if (wheelTurn)
        {
            float yRotation = steerAngle.Value - wheelMesh.localEulerAngles.z;
            wheelMesh.localEulerAngles = new Vector3(
                wheelMesh.localEulerAngles.x,
                yRotation,
                wheelMesh.localEulerAngles.z
            );
        }

        wheelMesh.Rotate(wheelRpm.Value / 60f * 360f * Time.deltaTime, 0, 0);
    }
}