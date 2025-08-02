using UnityEngine;

public class LapTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Something entered the finish line: " + other.name);

        LapTracker tracker = other.GetComponentInParent<LapTracker>();
        if (tracker != null)
        {
            Debug.Log("LapTracker found on: " + other.name);
            tracker.PassFinishLine();
        }
        else
        {
            Debug.Log("No LapTracker found on: " + other.name);
        }
    }
}