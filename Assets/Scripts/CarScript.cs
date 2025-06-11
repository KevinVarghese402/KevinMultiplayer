using UnityEngine;
using Unity.Netcode;

public class CarScript : NetworkBehaviour
{
    public Rigidbody rigid;
    public WheelCollider wheel1, wheel2, Steeringwheel3, Steeringwheel4;
    private CarControls carcontrols;

    private float verticalInput;
    private float horizontalInput;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        carcontrols = GetComponent<CarControls>();
    }

    private void FixedUpdate()
    {
        if (!IsServer) return;
        ApplyMovement_Rpc();
    }

    
    [Rpc(SendTo.ClientsAndHost, RequireOwnership = false)]
    private void ApplyMovement_Rpc()
    {
        float motor = verticalInput * carcontrols.driveSpeed;
        wheel1.motorTorque = motor;
        wheel2.motorTorque = motor;
        Steeringwheel3.motorTorque = motor;
        Steeringwheel4.motorTorque = motor;

        float steer = carcontrols.steerSpeed * horizontalInput;
        wheel1.steerAngle = steer;
        wheel2.steerAngle = steer;
    }

    public void ReceiveInput(float vInput, float hInput)
    {
        verticalInput = vInput;
        horizontalInput = hInput;
    }
}