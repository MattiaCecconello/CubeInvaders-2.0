using UnityEngine;
using redd096;

[AddComponentMenu("Cube Invaders/Turret Graphics/Turret Shooter Graphics")]
public class TurretShooterGraphics : TurretGraphics
{
    [Header("Shooter")]
    [SerializeField] ParticleSystem fireVFX = default;
    [SerializeField] AudioStruct fireAudio = default;

    TurretShooter turretShooter;

    protected override void Awake()
    {
        base.Awake();

        //get logic component as turret shooter
        turretShooter = buildableObject as TurretShooter;
    }

    protected override Enemy GetEnemy()
    {
        //get enemy from logic component
        return turretShooter.EnemyToAttack;
    }

    #region events

    protected override void AddEvents()
    {
        base.AddEvents();

        turretShooter.onShoot += OnShoot;
    }

    protected override void RemoveEvents()
    {
        base.RemoveEvents();

        turretShooter.onShoot -= OnShoot;
    }

    void OnShoot(Transform shotSpawn)
    {
        //vfx on shoot
        ParticlesManager.instance.Play(fireVFX, shotSpawn.position, shotSpawn.rotation);
        SoundManager.instance.Play(fireAudio.audioClip, shotSpawn.position, fireAudio.volume);
    }

    #endregion

}
