using UnityEngine;
using UnityEngine.Rendering;

public class UIChange : MonoBehaviour
{
    public GameObject buttonPanel; 
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            buttonPanel.SetActive(false);
        }
       
    }


}
