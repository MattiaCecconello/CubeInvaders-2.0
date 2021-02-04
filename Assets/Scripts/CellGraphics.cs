﻿using UnityEngine;
using redd096;

[AddComponentMenu("Cube Invaders/World/Cell Graphics")]
public class CellGraphics : MonoBehaviour
{
    [Header("VFX")]
    [SerializeField] ParticleSystem rotationParticlePrefab = default;
    [SerializeField] AudioClip rotationSound = default;
    [SerializeField] ParticleSystem explosionCellPrefab = default;
    [SerializeField] AudioClip explosionCellSound = default;

    Pooling<ParticleSystem> poolRotationParticles = new Pooling<ParticleSystem>();
    Pooling<AudioSource> poolRotationSound = new Pooling<AudioSource>();
    Pooling<ParticleSystem> poolExplosionCell = new Pooling<ParticleSystem>();
    Pooling<AudioSource> poolExplosionSound = new Pooling<AudioSource>();

    CameraShake camShake;
    Cell cell;

    private void Awake()
    {
        camShake = FindObjectOfType<CameraShake>();
        cell = GetComponent<Cell>();

        cell.onWorldRotate += OnWorldRotate;
        cell.onDestroyCell += OnDestroyCell;
    }

    private void OnDestroy()
    {
        if(cell)
        {
            cell.onWorldRotate -= OnWorldRotate;
            cell.onDestroyCell -= OnDestroyCell;
        }
    }

    void OnWorldRotate(Coordinates coordinates)
    {
        ParticlesManager.instance.Play(poolRotationParticles, rotationParticlePrefab, transform.position, transform.rotation);
        SoundManager.instance.Play(poolRotationSound, rotationSound, transform.position);
    }

    void OnDestroyCell()
    {
        ParticlesManager.instance.Play(poolExplosionCell, explosionCellPrefab, transform.position, transform.rotation);
        SoundManager.instance.Play(poolExplosionSound, explosionCellSound, transform.position);
    }
}
