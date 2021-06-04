using UnityEngine;
using redd096;

[AddComponentMenu("Cube Invaders/Enemy Graphics/Enemy Soulbind Graphics")]
public class EnemySoulbindGraphics : MonoBehaviour
{
    [Header("SoulBind")]
    [SerializeField] bool lookAtSoulBind = false;
    [CanShow("lookAtSoulBind")] [SerializeField] Transform objectToRotateToSoulbind = default;

    [Header("Outline")]
    [SerializeField] Outline outlineObject = default;

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
        LookAtSoulbind();

        //show outline when no one is aiming, else disable
        ShowHideOutline();
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

    void LookAtSoulbind()
    {
        //look at soulbind
        if (lookAtSoulBind && objectToRotateToSoulbind && enemy.soulBind)
            objectToRotateToSoulbind.rotation = Quaternion.LookRotation(enemy.soulBind.transform.position - transform.position);
    }

    void ShowHideOutline()
    {
        //show outline when no one is aiming, else disable
        if (outlineObject)
        {
            //hide if aimed
            bool hide = enemy.turretsAiming != null && enemy.turretsAiming.Length > 0;

            //if (outlineObject.enabled != !hide)
                outlineObject.enabled = !hide;
        }
    }
}
