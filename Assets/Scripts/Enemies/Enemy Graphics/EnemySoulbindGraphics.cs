using UnityEngine;
using redd096;

[AddComponentMenu("Cube Invaders/Enemy Graphics/Enemy Soulbind Graphics")]
public class EnemySoulbindGraphics : MonoBehaviour
{
    [Header("SoulBind")]
    [SerializeField] bool lookAtSoulBind = false;

    [Header("VFX")]
    [SerializeField] ParticleSystem particlesSpawnFirstSoulbind = default;
    [SerializeField] AudioStruct soundSpawnFirstSoulbind = default;
    [SerializeField] ParticleSystem particlesSpawnSecondSoulbind = default;
    [SerializeField] AudioStruct soundSpawnSecondSoulbind = default;

    EnemySoulbind enemy;

    void OnEnable()
    {
        enemy = GetComponent<EnemySoulbind>();
        enemy.onSpawnSoulbind += OnSpawnSoulbind;
    }

    void OnDisable()
    {
        if (enemy)
        {
            enemy.onSpawnSoulbind -= OnSpawnSoulbind;
        }
    }

    void FixedUpdate()
    {
        //look at soulbind
        if(lookAtSoulBind)
            transform.rotation = Quaternion.LookRotation(enemy.soulBind.transform.position - transform.position);
    }

    void OnSpawnSoulbind(Vector3 firstPosition, Quaternion firstRotation, Vector3 secondPosition, Quaternion secondRotation)
    {
        //previous
        ParticlesManager.instance.Play(particlesSpawnFirstSoulbind, firstPosition, firstRotation);
        SoundManager.instance.Play(soundSpawnFirstSoulbind.audioClip, firstPosition, soundSpawnFirstSoulbind.volume);

        //new
        ParticlesManager.instance.Play(particlesSpawnSecondSoulbind, secondPosition, secondRotation);
        SoundManager.instance.Play(soundSpawnSecondSoulbind.audioClip, secondPosition, soundSpawnSecondSoulbind.volume);
    }
}
