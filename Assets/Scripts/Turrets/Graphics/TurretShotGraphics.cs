using UnityEngine;
using redd096;

[AddComponentMenu("Cube Invaders/Turret Graphics/Turret Shot Graphics")]
public class TurretShotGraphics : MonoBehaviour
{
    [Header("Trail")]
    [SerializeField] TrailRenderer trail = default;

    [Header("On Hit Enemy")]
    [SerializeField] ParticleSystem particlesOnHit = default;

    [Header("On Autodestruction")]
    [SerializeField] ParticleSystem particlesOnAutodestruction = default;

    TurretShot turretShot;

    void OnEnable()
    {
        //get reference
        turretShot = GetComponent<TurretShot>();

        //add events
        if(turretShot)
        {
            turretShot.onInit += OnInit;
            turretShot.onDestroyShot += OnDestroyShot;
        }
    }

    void OnDisable()
    {
        //remove events
        if (turretShot)
        {
            turretShot.onInit -= OnInit;
            turretShot.onDestroyShot -= OnDestroyShot;
        }
    }

    void OnInit()
    {
        //reset trail
        if (trail)
            trail.Clear();
    }

    void OnDestroyShot(bool hitEnemy)
    {
        //instantiate particles on hit or autodestruction
        ParticlesManager.instance.Play(hitEnemy ? particlesOnHit : particlesOnAutodestruction, transform.position, transform.rotation);
    }
}
