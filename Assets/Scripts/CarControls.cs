using UnityEngine;
using Unity.Netcode;

public class CarControls : NetworkBehaviour
{
    public float driveSpeed = 900f;
    public float steerSpeed = 30f;

    public float VerticalInput { get; private set; }
    public float HorizontalInput { get; private set; }


    private void Update()
    {
        if (!IsLocalPlayer) return;
        
        VerticalInput = Input.GetAxis("Vertical");
        HorizontalInput = Input.GetAxis("Horizontal");
        bool driftInput = Input.GetKey(KeyCode.Space);
        
        SubmitInputsServerRpc(VerticalInput, HorizontalInput, driftInput);
    }
    
    [ServerRpc]
    private void SubmitInputsServerRpc(float vInput, float hInput, bool driftinput)
    {
        var car = GetComponent<CarScript>();
        car.ReceiveInput(vInput, hInput, driftinput);
    }
}