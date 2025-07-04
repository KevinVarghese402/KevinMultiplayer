using UnityEngine;

public class BoosterTile : MonoBehaviour
{
    public float boostForce = 10f;

    private void OnTriggerEnter(Collider other)
    {
        BoostableCar boostable = other.GetComponentInParent<BoostableCar>();
        if (boostable != null)
        {
            boostable.Boost(boostForce);
        }
    }
}ggi