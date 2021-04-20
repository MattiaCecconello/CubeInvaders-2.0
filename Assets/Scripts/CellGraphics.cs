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

    [Header("Radar things")]
    [SerializeField] Transform enemyDestinationObject = default;

    CameraShake camShake;
    Cell cell;

    Enemy nearestEnemy;

    private void OnEnable()
    {
        camShake = FindObjectOfType<CameraShake>();
        cell = GetComponent<Cell>();

        if (cell)
        {
            cell.onWorldRotate += OnWorldRotate;
            cell.onDestroyCell += OnDestroyCell;
            cell.onShowEnemyDestination += OnShowEnemyDestination;
            cell.onHideEnemyDestination += OnHideEnemyDestination;
        }

        //by default, hide enemy destination
        OnHideEnemyDestination();
    }

    private void OnDisable()
    {
        if(cell)
        {
            cell.onWorldRotate -= OnWorldRotate;
            cell.onDestroyCell -= OnDestroyCell;
            cell.onShowEnemyDestination -= OnShowEnemyDestination;
            cell.onHideEnemyDestination -= OnHideEnemyDestination;
        }
    }

    void Update()
    {
        //update size enemy destination object
        UpdateDestinationObject();
    }

    #region events

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

    void OnShowEnemyDestination(Enemy nearestEnemy)
    {
        //show destination
        if (GameManager.instance.levelManager.generalConfig.showEnemiesDestination)
        {
            enemyDestinationObject?.gameObject.SetActive(true);

            //set nearest enemy
            this.nearestEnemy = nearestEnemy;
        }
    }

    void OnHideEnemyDestination()
    {
        //hide destination
        enemyDestinationObject?.gameObject.SetActive(false);

        //be sure to start from 0 when reactivate
        if(enemyDestinationObject)
            enemyDestinationObject.transform.localScale = new Vector3(0, 0, 0);

        //remove nearest enemy
        nearestEnemy = null;
    }

    #endregion

    #region private API

    void UpdateDestinationObject()
    {
        //update destination object
        if (enemyDestinationObject && nearestEnemy)
        {
            //set size based on distance from cube
            float distanceFrom0To1 = 1 - (nearestEnemy.DistanceFromCube / GameManager.instance.levelManager.generalConfig.minDistanceToShowDestination);    //distance from 0 to 1
            float size = Mathf.Lerp(0, 1, distanceFrom0To1);

            //show only if distance greater than minimum
            if (nearestEnemy.DistanceFromCube > GameManager.instance.levelManager.generalConfig.minDistanceToShowDestination)
                size = 0;

            //set size
            enemyDestinationObject.transform.localScale = new Vector3(size, size, size);
        }
    }

    #endregion
}
