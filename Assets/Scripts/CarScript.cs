using UnityEngine;
using Unity.Netcode;

public class CarScript : NetworkBehaviour
{
    public Rigidbody rigid;
    public Vector3 centerOfMassOfCar = new Vector3(0f, -1.0f, -0.8f);
    public WheelCollider wheel1, wheel2, Steeringwheel3, Steeringwheel4;
    private CarControls carcontrols;
    private bool isDrifting = false;
    
    //reset timer
    private float upsideDownTimer = 0f;
    private float resetDelay = 2f; // seconds before reset
    
    //Drifting Marks
    public TrailRenderer[] tyreMarks;
    
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

        if (IsServer)
        {
            CheckIfUpsideDown();
        }
        
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

        if (isDrifting == true)
        {
            ApplyGrip(wheel1, sideStiffness:0.2f, forwardStiffness:0.2f);
            ApplyGrip(wheel2, sideStiffness:0.2f, forwardStiffness:0.2f);
            //front wheels
            ApplyGrip(Steeringwheel3, sideStiffness:1.2f, forwardStiffness:1.8f);
            ApplyGrip(Steeringwheel4, sideStiffness:1.2f, forwardStiffness:1.8f);
            startEmmiter();
        }
        else
        {
            ApplyGrip(wheel1, sideStiffness: 1.0f, forwardStiffness: 1.3f);
            ApplyGrip(wheel2, sideStiffness: 1.0f, forwardStiffness: 1.3f);
            //front wheels
            ApplyGrip(Steeringwheel3, sideStiffness: 1.2f, forwardStiffness: 1.4f);
            ApplyGrip(Steeringwheel4, sideStiffness: 1.2f, forwardStiffness: 1.4f);
            stopEmmiter();
        }
    }

    public void ReceiveInput(float vInput, float hInput, bool driftinput)
    {
        verticalInput = vInput;
        horizontalInput = hInput;
        isDrifting = driftinput; 
    }
    private void ApplyGrip(WheelCollider wheels, float sideStiffness, float forwardStiffness)
    {
        WheelFrictionCurve sideFriction = Steeringwheel3.sidewaysFriction;
        sideFriction.extremumValue = 2.0f;
        sideFriction.asymptoteValue = 1.0f;
        sideFriction.stiffness = sideStiffness;
        wheels.sidewaysFriction = sideFriction;
        

        WheelFrictionCurve forwardFriction = Steeringwheel3.forwardFriction;
        forwardFriction.extremumValue = 2.0f;
        forwardFriction.asymptoteValue = 1.0f;
        forwardFriction.stiffness = forwardStiffness;
        wheels.forwardFriction = forwardFriction;
        
    }

    private void CheckIfUpsideDown()
    {
        if (Vector3.Dot(transform.up, Vector3.up) < 0.1f)
        {
            upsideDownTimer += Time.fixedDeltaTime;

            if (upsideDownTimer >= resetDelay)
            {
                ResetCarUpright();
                upsideDownTimer = 0f;
            }
        }
        else
        {
            upsideDownTimer = 0f; // reset if not upside down
        }
    }
    private void ResetCarUpright()
    {
        Vector3 currentEuler = transform.eulerAngles;
        transform.eulerAngles = new Vector3(0, currentEuler.y, 0);
        
        transform.position += Vector3.up * 1.5f;
        
        rigid.linearVelocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
    }

    private void startEmmiter()
    {
        foreach (TrailRenderer T in tyreMarks)
        {
            T.emitting = true;
        }
    }

    private void stopEmmiter()
    {
        foreach (TrailRenderer T in tyreMarks)
        {
            T.emitting = false;
        }
    }
}