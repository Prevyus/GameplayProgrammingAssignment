using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ColliderDetector
{
    public CollisionDetector collider;
    public float mult;
}

public class ObjectHealthComponent : HealthComponent
{ // HEALTH COMPONENT THAT KEEPS TRACK OF VARIOUS COLLIDERS INSIDE THIS OBJECT AND TAKES DAMAGE FROM THEM GETTING HIT
    [Header("Collision")]
    public List<ColliderDetector> colliders = new List<ColliderDetector>();

    public override void Start()
    {
        base.Start();

        for (int i = 0; i < colliders.Count; i++)
        {
            int index = i;
            colliders[index].collider.OnHit += (damage) => DealDamage(damage, index);
        }
    }

    public virtual void DealDamage(float damage, int colliderIndex)
    {
        if (canTakeDamage)
        {
            currentHealth = Mathf.Clamp(currentHealth - (damage * colliders[colliderIndex].mult), 0, maxHealth);
            InvokeOnDealtDamage();
        }
    }

    public override void Death()
    {
        base.Death();

        Destroy(gameObject);
    }

    public override void Revive()
    {
        base.Revive();

    }
}
