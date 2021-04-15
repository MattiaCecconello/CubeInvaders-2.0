using UnityEngine;
using redd096;

[AddComponentMenu("Cube Invaders/World/Cell Graphics")]
public class CellGraphics : MonoBehaviour
{
    [Header("VFX")]
    [SerializeField] ParticleSystem rotationParticlePrefab = default;
    [SerializeField] AudioStruct rotationSound = default;
    [SerializeField] ParticleSystem explosionCellPrefab = default;
    [SerializeField] AudioStruct explosionCellSound = default;

    CameraShake camShake;
    Cell cell;

    private void Start()
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
        ParticlesManager.instance.Play(rotationParticlePrefab, transform.position, transform.rotation);
        SoundManager.instance.Play(rotationSound.audioClip, transform.position, rotationSound.volume);
    }

    void OnDestroyCell()
    {
        ParticlesManager.instance.Play(explosionCellPrefab, transform.position, transform.rotation);
        SoundManager.instance.Play(explosionCellSound.audioClip, transform.position, explosionCellSound.volume);

        //do camera shake
        camShake.DoShake();
    }
}
