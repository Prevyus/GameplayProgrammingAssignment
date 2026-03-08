using UnityEngine;
using System;

public class HealthComponent : MonoBehaviour
{ // HEALTH COMPONENT THAT YOU CAN USE ON ANYTHING
    [Header("Stats")]
    public float maxHealth = 100;
    public float currentHealth = 100;
    public float reviveHealth = 10;
    [HideInInspector] public float Health => maxHealth > 0f ? currentHealth / maxHealth : 0f;
    public bool canTakeDamage = true;
    public bool canDie = true;
    public bool isAlive = true;

    public event Action OnMaxHealth; public void InvokeOnMaxHealth() { OnMaxHealth?.Invoke(); }
    public event Action OnDealtDamage; public void InvokeOnDealtDamage() { OnDealtDamage?.Invoke(); }
    public event Action OnRegenerated; public void InvokeOnRegenerated() { OnRegenerated?.Invoke(); }
    public event Action OnDeath; public void InvokeOnDeath() { OnDeath?.Invoke(); }
    public event Action OnRevive; public void InvokeOnRevive() { OnRevive?.Invoke(); }

    float oldHealth = 0f;

    public virtual void ResetComponent()
    {
        currentHealth = maxHealth;
        canTakeDamage = true;
        canDie = true;
        isAlive = true;
    }

    public virtual void Start()
    {
        //ResetComponent();
    }

    private void Update()
    {// CONSTANTLY CHECKS IF ITS DEAD OR ALIVE
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (oldHealth != currentHealth)
        {
            currentHealth = Health * maxHealth;

            oldHealth = currentHealth;

            if (currentHealth <= 0) Death();
            else if (currentHealth > 0 && !isAlive) Revive();
        }
    }

    public virtual void DealDamage(float damage)
    {
        if (canTakeDamage) 
        { 
            currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
            InvokeOnDealtDamage();
        }
    }

    public void Regenerate(float health)
    {
        currentHealth = Mathf.Clamp(currentHealth + health, 0, maxHealth);
        InvokeOnRegenerated();
        if (currentHealth >= maxHealth) InvokeOnMaxHealth();
    }

    public virtual void Death()
    {
        if (canDie)
        {
            isAlive = false;
            InvokeOnDeath();
        }
    }

    public virtual void Revive()
    {
        if (canDie)
        {
            isAlive = true;
            InvokeOnRevive();
        }
    }
}
