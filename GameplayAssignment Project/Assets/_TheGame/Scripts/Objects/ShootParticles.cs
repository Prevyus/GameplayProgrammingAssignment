using UnityEngine;

public class ShootParticles : MonoBehaviour
{// SMALL CONTROLLER USED FOR THE GUNFIRE PARTICLES
    public ParticleSystem red;
    public ParticleSystem orange;

    public void Shoot()
    {
        red.Play();
        orange.Play();
    }
}
