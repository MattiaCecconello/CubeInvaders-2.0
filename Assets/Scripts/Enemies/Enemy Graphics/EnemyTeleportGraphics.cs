using UnityEngine;
using redd096;

[AddComponentMenu("Cube Invaders/Enemy Graphics/Enemy Teleport Graphics")]
public class EnemyTeleportGraphics : MonoBehaviour
{
    [Header("VFX")]
    [SerializeField] ParticleSystem teleportPreviousPositionParticlePrefab = default;
    [SerializeField] AudioStruct teleportPreviousPositionSound = default;
    [SerializeField] ParticleSystem teleportNewPositionParticlePrefab = default;
    [SerializeField] AudioStruct teleportNewPositionSound = default;

    EnemyTeleport enemyTeleport;

    void OnEnable()
    {
        enemyTeleport = GetComponent<EnemyTeleport>();
        enemyTeleport.onTeleport += OnTeleport;
    }

    void OnDisable()
    {
        if (enemyTeleport)
        {
            enemyTeleport.onTeleport -= OnTeleport;
        }
    }

    void OnTeleport(Vector3 previousPosition, Quaternion previousRotation, Vector3 newPosition, Quaternion newRotation)
    {
        //previous
        ParticlesManager.instance.Play(teleportPreviousPositionParticlePrefab, previousPosition, previousRotation);
        SoundManager.instance.Play(teleportPreviousPositionSound.audioClip, previousPosition, teleportPreviousPositionSound.volume);

        //new
        ParticlesManager.instance.Play(teleportNewPositionParticlePrefab, newPosition, newRotation);
        SoundManager.instance.Play(teleportNewPositionSound.audioClip, newPosition, teleportNewPositionSound.volume);
    }
}
