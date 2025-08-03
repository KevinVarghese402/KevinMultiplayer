using Unity.Netcode;
using UnityEngine;

public class Wheel : NetworkBehaviour
{
    public WheelCollider wheelCollider;
    public Transform wheelMesh;
    public bool wheelTurn; // Whether this wheel is a steering wheel

    // Network variables for synchronization from server to clients
    private NetworkVariable<float> syncedRPM = new NetworkVariable<float>(
        0f, 
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Server);
    
    private NetworkVariable<float> syncedSteerAngle = new NetworkVariable<float>(
        0f, 
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Server);

    
    [SerializeField] private float networkUpdateInterval = 0.1f; // e.g., update 10 times per second
    private float lastNetworkUpdateTime;
    
    [SerializeField] private float steerAngleThreshold = 0.5f; // Only send if steer angle changes by more than 0.5 degrees

    private float _lastSentSteerAngle; 
    private void Start()
    {
 
        _lastSentSteerAngle = wheelCollider.steerAngle;
      
    }

    private void Update()
    {
    
        if (IsServer)
        {
            // Check if enough time has passed since the last network update
            if (Time.time - lastNetworkUpdateTime >= networkUpdateInterval)
            {
                syncedRPM.Value = wheelCollider.rpm;
                
                if (wheelTurn)
                {
                    float currentSteerAngle = wheelCollider.steerAngle;
                    if (Mathf.Abs(currentSteerAngle - _lastSentSteerAngle) > steerAngleThreshold)
                    {
                        syncedSteerAngle.Value = currentSteerAngle;
                        _lastSentSteerAngle = currentSteerAngle; 
                    }
                }
                
                lastNetworkUpdateTime = Time.time; // Reset the timer
            }
        }
        
        RotateWheelMesh(syncedRPM.Value); 

        if (wheelTurn)
        {

            Vector3 currentLocalEuler = wheelMesh.localEulerAngles;
            wheelMesh.localEulerAngles = new Vector3(
                currentLocalEuler.x,          
                syncedSteerAngle.Value,       
                currentLocalEuler.z           
            );

        }
    }
    private void RotateWheelMesh(float rpm)
    {
        // Calculate the rotation speed in degrees per frame for the X-axis (around the axle)
        float rotationSpeed = rpm / 60f * 360f * Time.deltaTime;
        wheelMesh.Rotate(rotationSpeed, 0, 0); // Rotate around its local X-axis
    }
}