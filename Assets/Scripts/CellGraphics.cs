using UnityEngine;
using redd096;

[AddComponentMenu("Cube Invaders/World/Cell Graphics")]
public class CellGraphics : MonoBehaviour
{
    [Header("VFX")]
    [SerializeField] ParticleSystem rotationParticlePrefab = default;
    [SerializeField] AudioStruct rotationSound = default;
    [SerializeField] GameObject explosionShaderGraph = default;
    [SerializeField] ParticleSystem explosionCellPrefab = default;
    [SerializeField] AudioStruct explosionCellSound = default;

    [Header("Radar things")]
    [SerializeField] SpriteRenderer enemyDestinationObject = default;
    [SerializeField] float flickSpeed = 1;

    [Header("Show Sprites")]
    [SerializeField] GameObject spriteToShow = default;

    Cell cell;

    Enemy nearestEnemy;

    private void OnEnable()
    {
        cell = GetComponent<Cell>();

        //add events
        if (cell)
        {
            cell.onWorldRotate += OnWorldRotate;
            cell.onDestroyCell += OnDestroyCell;
            cell.onShowEnemyDestination += OnShowEnemyDestination;
            cell.onHideEnemyDestination += OnHideEnemyDestination;
        }

        GameManager.instance.levelManager.onSetBuildMode += OnSetBuildMode;

        //by default, hide enemy destination
        OnHideEnemyDestination();

        //by default, hide sprite
        if (spriteToShow)
            spriteToShow.SetActive(false);
    }

    private void OnDisable()
    {
        //remove events
        if(cell)
        {
            cell.onWorldRotate -= OnWorldRotate;
            cell.onDestroyCell -= OnDestroyCell;
            cell.onShowEnemyDestination -= OnShowEnemyDestination;
            cell.onHideEnemyDestination -= OnHideEnemyDestination;
        }

        GameManager.instance.levelManager.onSetBuildMode -= OnSetBuildMode;
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

    public void OnDestroyCell()
    {
        //esplosione da shader graph
        if(explosionShaderGraph)
        {
            explosionShaderGraph.SetActive(false);
            explosionShaderGraph.SetActive(true);
        }

        //ParticlesManager.instance.Play(explosionCellPrefab, transform.position, transform.rotation);
        SoundManager.instance.Play(explosionCellSound.audioClip, transform.position, explosionCellSound.volume);

        //do camera shake
        GameManager.instance.cameraShake.DoShake();
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

        //set alpha to 0, for when reactivate it
        enemyDestinationObject.color = new Color(enemyDestinationObject.color.r, enemyDestinationObject.color.g, enemyDestinationObject.color.b, 0);

        //remove nearest enemy
        nearestEnemy = null;
    }

    void OnSetBuildMode(bool isOpening)
    {
        //show or hide sprites
        if(spriteToShow)
        {
            //show only if is opening and option is on
            if(spriteToShow.activeInHierarchy != (isOpening && GameManager.instance.ShowSpritesOption))
                spriteToShow.SetActive(!spriteToShow.activeInHierarchy);
        }
    }

    #endregion

    #region private API

    void UpdateDestinationObject()
    {
        //update destination object
        if (enemyDestinationObject && nearestEnemy)
        {
            float alpha = Mathf.Abs(Mathf.Sin(Time.time * flickSpeed));

            //set alpha
            enemyDestinationObject.color = new Color(enemyDestinationObject.color.r, enemyDestinationObject.color.g, enemyDestinationObject.color.b, alpha);
        }
    }

    #endregion
}
