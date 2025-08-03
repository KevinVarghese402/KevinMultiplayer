using System;
using UnityEngine;
using Unity.Netcode;

public class CarScript : NetworkBehaviour
{
    public Rigidbody rigid;
    public Vector3 centerOfMassOfCar = new Vector3(0f, -1.0f, -0.8f);
    public WheelCollider wheel1, wheel2, Steeringwheel3, Steeringwheel4;
    private CarControls carcontrols;
    private bool isDrifting = false;
    
    [Header("Engine Audio")]
    public AudioSource engineAudioSource;
    public AudioClip engineClip;
    public float minPitch = 0.8f;
    public float maxPitch = 2.0f;
    
    [Header("Drifting Audio")]
    public AudioSource DriftingSource;
    public AudioClip driftingClip;
    
    // Reset timer
    private float upsideDownTimer = 0f;
    private float resetDelay = 2f; 
    
    // Drifting marks
    public TrailRenderer[] tyreMarks;
    
    [SerializeField] private GameObject cameraObject;
    
    // Inputs received from the controls (updated by CarControls server RPC)
    private float verticalInput;
    private float horizontalInput;

    // Store last input sent to clients to minimize redundant network calls
    private float lastVerticalInput = 0f;
    private float lastHorizontalInput = 0f;
    private bool lastIsDrifting = false;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        carcontrols = GetComponent<CarControls>();

        if (IsLocalPlayer) cameraObject.SetActive(true);

        if (IsLocalPlayer && rigid != null)
        {
            rigid.centerOfMass += centerOfMassOfCar;
        }

        if (IsLocalPlayer && engineAudioSource != null && engineClip != null)
        {
            engineAudioSource.clip = engineClip;
            engineAudioSource.loop = true;
            engineAudioSource.volume = 0.03f;
            engineAudioSource.Play();
        }
    }

    private void FixedUpdate()
    {
        if (!IsServer) return;

        // Send movement RPC to clients only if input changed to avoid flooding network
        if (verticalInput != lastVerticalInput || horizontalInput != lastHorizontalInput || isDrifting != lastIsDrifting)
        {
            ApplyMovement_Rpc();

            lastVerticalInput = verticalInput;
            lastHorizontalInput = horizontalInput;
            lastIsDrifting = isDrifting;

            
        }

        CheckIfUpsideDown();

        if (IsLocalPlayer && engineAudioSource != null)
        {
            float speed = rigid.linearVelocity.magnitude; // Use rigid.velocity for current velocity
            engineAudioSource.pitch = Mathf.Lerp(minPitch, maxPitch, speed / 20f);
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

        if (isDrifting && Mathf.Abs(horizontalInput) > 0.1f)
        {
            startEmitter();
            
            float direction = Mathf.Sign(horizontalInput);
            Vector3 rearPosition = transform.position - transform.forward * 1.5f;
            Vector3 forceDirection = transform.right * direction;
            rigid.AddForceAtPosition(forceDirection * 50f, rearPosition, ForceMode.Impulse);
            DriftingSource.PlayOneShot(driftingClip);
        }
        else
        {
            stopEmitter();
        }
    }

    // Called from CarControls' ServerRpc when client sends input
    public void ReceiveInput(float vInput, float hInput, bool driftInput)
    {
        verticalInput = vInput;
        horizontalInput = hInput;
        isDrifting = driftInput;
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
            upsideDownTimer = 0f;
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

    private void startEmitter()
    {
        foreach (TrailRenderer T in tyreMarks)
        {
            T.emitting = true;
        }
    }

    private void stopEmitter()
    {
        foreach (TrailRenderer T in tyreMarks)
        {
            T.emitting = false;
        }
    }
}
