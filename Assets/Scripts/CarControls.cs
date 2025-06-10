using UnityEngine;
using Unity.Netcode;

public class CarControls : NetworkBehaviour
{
    public float driveSpeed = 1500f;
    public float steerSpeed = 30f;

    public float VerticalInput { get; private set; }
    public float HorizontalInput { get; private set; }

    private void Update()
    {
        if (!IsLocalPlayer) return;

        VerticalInput = Input.GetAxis("Vertical");
        HorizontalInput = Input.GetAxis("Horizontal");
    }
}