using UnityEngine;
using Unity.Netcode;

public class CarScript : NetworkBehaviour
{
    public Rigidbody rigid;
    public Vector3 centerOfMassOfCar = new Vector3(0f, -0.5f, 0f);
    public WheelCollider wheel1, wheel2, Steeringwheel3, Steeringwheel4;
    private CarControls carcontrols;
    private bool isDrifting = false;
    [SerializeField] private GameObject cameraObject;
    
    private float verticalInput;
    private float horizontalInput;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        carcontrols = GetComponent<CarControls>();
        if(IsLocalPlayer) cameraObject.SetActive(true);
        
        if (IsLocalPlayer && rigid != null)
        {
            rigid.centerOfMass += centerOfMassOfCar;
        }
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
        
        Steeringwheel3.steerAngle = steer;
        Steeringwheel4.steerAngle = steer;

        ApplyFriction(isDrifting); 
    }

    public void ReceiveInput(float vInput, float hInput, bool driftinput)
    {
        verticalInput = vInput;
        horizontalInput = hInput;
        isDrifting = driftinput; 
    }
    private void ApplyFriction(bool drifting)
    {
        WheelFrictionCurve rearFriction = Steeringwheel3.sidewaysFriction;
      

        if (drifting)
        {
            rearFriction.extremumValue = 0.2f;
            rearFriction.asymptoteValue = 0.1f;
            rearFriction.stiffness = 0.4f;   
        }
        else
        {
            rearFriction.extremumValue = 2.0f;
            rearFriction.asymptoteValue = 1.0f;
            rearFriction.stiffness = 1.0f;
        }
        //fwd so keeping the front 2 gripped
        wheel1.sidewaysFriction = rearFriction;
        wheel2.sidewaysFriction = rearFriction;
        
    }
}