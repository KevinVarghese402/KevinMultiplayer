using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

//this stayes on the server
public class SphereChangling : NetworkBehaviour
{
    [SerializeField]
    bool big = true;
    
    [SerializeField]
    NetworkVariable<int> dangerlevel = new NetworkVariable<int>(1);
    
    private void FixedUpdate()
    {
        if (IsServer)
        {
            if (Random.value > 0.9f)
            {
                ChangeSize_Rpc(!big);
                Color _ballColor = new Color(Random.value, Random.value, Random.value);
                ChangeBall(_ballColor);
            }   
            dangerlevel.Value = Random.Range (0, 100);
        }
        
    }
    //Remote Procedure (Function) Call
    [Rpc(SendTo.ClientsAndHost, Delivery = RpcDelivery.Reliable)]
    public void ChangeSize_Rpc(bool _big)
    {
        Debug.Log("IsHost = " + IsHost);
        Debug.Log("IsClient = " + IsClient);
        Debug.Log("IsServer =" + IsServer);
        Debug.Log("IsLocalPlayer = " + IsLocalPlayer);
        Debug.Log("IsOwner = " + IsOwner);
        
        this.big = _big;
        
        // a visual effect
        if (_big)
            transform.localScale = new Vector3(2f, 2f, 2f);
        else 
            transform.localScale = new Vector3(1f, 1f, 1f);
    }
    
    private void ChangeBall(Color _ballColor) // cant be a bool 
    {
        GetComponent<MeshRenderer>().material.color = _ballColor;
    }
}
