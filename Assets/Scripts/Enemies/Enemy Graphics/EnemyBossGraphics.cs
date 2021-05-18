using UnityEngine;
using redd096;

[AddComponentMenu("Cube Invaders/Enemy Graphics/Enemy Boss Graphics")]
public class EnemyBossGraphics : MonoBehaviour
{
    [Header("On Boss Death")]
    [SerializeField] ParticleSystem particlesOnDeath = default;
    [SerializeField] AudioStruct audioOnDeath = default;

    [Header("Camera Shake on Death")]
    [SerializeField] bool shake = false;
    [CanShow("shake")] [SerializeField] bool useCustomShake = false;
    [CanShow("shake", "useCustomShake")] [SerializeField] float amplitudeGain = 1;
    [CanShow("shake", "useCustomShake")] [SerializeField] float frequemcyGain = 1;
    [CanShow("shake", "useCustomShake")] [SerializeField] float shakeDuration = 1;

    EnemyBoss enemy;

    private void OnEnable()
    {
        //add events
        enemy = GetComponent<EnemyBoss>();
        if (enemy)
        {
            enemy.onEnemyDeath += OnEnemyDeath;
        }
    }

    private void OnDisable()
    {
        //remove events
        if(enemy)
        {
            enemy.onEnemyDeath -= OnEnemyDeath;
        }
    }

    void OnEnemyDeath(Enemy enemy)
    {
        //vfx and sound
        ParticlesManager.instance.Play(particlesOnDeath, transform.position, transform.rotation);
        SoundManager.instance.Play(audioOnDeath.audioClip, transform.position, audioOnDeath.volume);

        //camera shake
        if(shake)
        {
            //use custom or normal shake
            if(useCustomShake)
                GameManager.instance.cameraShake.DoShake(amplitudeGain, frequemcyGain, shakeDuration);
            else
                GameManager.instance.cameraShake.DoShake();
        }
    }
}
