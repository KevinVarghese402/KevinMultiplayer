using UnityEngine;
using Unity.Netcode;
public class CarScript : NetworkBehaviour
{
    public Rigidbody rigid;
    public WheelCollider wheel1, wheel2, Steeringwheel3, Steeringwheel4;
    private CarControls carcontrols;

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
        float motor = carcontrols.VerticalInput * carcontrols.driveSpeed;
        wheel1.motorTorque = motor;
        wheel2.motorTorque = motor;
        Steeringwheel3.motorTorque = motor;
        Steeringwheel4.motorTorque = motor;
        
        float steer = carcontrols.steerSpeed * carcontrols.HorizontalInput;
        wheel1.steerAngle = steer;
        wheel2.steerAngle = steer;
    }
}