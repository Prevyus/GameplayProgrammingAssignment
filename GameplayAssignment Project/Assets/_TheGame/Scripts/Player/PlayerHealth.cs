using UnityEngine;

public class PlayerHealth : HealthComponent
{ // HEALTH COMPONENT FOR THE PLAYER

    public float damageToTake = 15;
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log($"{gameObject.name} | {other.gameObject.name}");
        DealDamage(damageToTake);
    }
}
