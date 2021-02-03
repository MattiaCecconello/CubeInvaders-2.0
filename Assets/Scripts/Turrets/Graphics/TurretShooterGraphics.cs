﻿using UnityEngine;
using redd096;

[AddComponentMenu("Cube Invaders/Turret Graphics/Turret Shooter Graphics")]
public class TurretShooterGraphics : TurretGraphics
{
    [Header("Shooter")]
    [SerializeField] ParticleSystem fireVFX = default;
    [SerializeField] AudioClip fireAudio = default;

    TurretShooter turretShooter;

    Pooling<ParticleSystem> poolFireVFX = new Pooling<ParticleSystem>();
    Pooling<AudioSource> poolFireAudio = new Pooling<AudioSource>();

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

    void OnShoot()
    {
        //vfx on shoot
        ParticlesManager.instance.Play(poolFireVFX, fireVFX, transform.position, transform.rotation);
        SoundManager.instance.Play(poolFireAudio, fireAudio, transform.position);
    }

    #endregion

}
