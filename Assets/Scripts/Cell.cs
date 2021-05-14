using UnityEngine;
using redd096;

[SelectionBase]
[AddComponentMenu("Cube Invaders/World/Cell")]
[RequireComponent(typeof(CellGraphics))]
public class Cell : MonoBehaviour
{
    [Header("Modifier")]
    [SerializeField] bool isInvincible = false;
    [SerializeField] bool onlyOneLife = false;

    [Header("Important")]
    [SerializeField] GameObject toRemoveOnDead = default;
    [SerializeField] BuildableObject turretToCreate = default;
    [SerializeField] bool buildTurretAtStart = false;
    [SerializeField] bool canRemoveTurret = true;

    [Header("Resources")]
    [SerializeField] float resourcesToRecreateCell = 100;
    [SerializeField] float resourcesToCreateTurret = 10;
    [SerializeField] float resourcesOnSellTurret = 10;

    [Header("Debug")]
    public Coordinates coordinates;
    [ReadOnly] public Coordinates startCoordinates;

    //used from turret to know when is rotating
    public System.Action<Coordinates> onWorldRotate;
    public System.Action onDestroyCell;
    public System.Action<Enemy> onShowEnemyDestination;
    public System.Action onHideEnemyDestination;

    public BuildableObject turret { get; private set; }

    public bool IsAlive { get; private set; } = true;

    public BuildableObject TurretToCreate => turretToCreate;

    void Awake()
    {
        //if build at start, build turret 
        BuildAtStart();
    }

    void OnDestroy()
    {
        //be sure to reset event
        onWorldRotate = null;
    }
    
    void OnTriggerEnter(Collider other)
    {
        //if hitted by enemy
        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy && enemy.StillAlive)  //be sure enemy is still alive (to not have 2 hit on same frame)
        {
            //only if not enemy poison, or is not active destroyCross (cause poison destroy this cell)
            if (enemy.GetType() != typeof(EnemyPoison) && enemy.DestroyCross == false)
            {
                //kill cell 
                KillCell();
            }

            //destroy enemy
            enemy.Die(this);
        }
    }

    #region private API

    void BuildAtStart()
    {
        //if build at start, build turret 
        if (buildTurretAtStart)
        {
            ShowPreview();
            BuildOnCell();
        }
    }

    void BuildOnCell()
    {
        //do only if there is a turret and is only a preview
        if (turret && turret.IsPreview)
        {
            //build it
            turret.BuildTurret(this);
        }
    }

    void RemoveBuildOnCell()
    {
        //do only if there is a turret and is not a preview
        if (turret && turret.IsPreview == false)
        {
            //remove it
            turret.RemoveTurret();
        }
    }

    void DestroyCell(bool loaded = false)
    {
        IsAlive = false;

        //remove turret
        RemoveBuildOnCell();

        //remove biome
        ActiveRemoveOnDead(false);

        //call feedback only when loaded is false
        if(loaded == false)
            onDestroyCell?.Invoke();
    }

    void RecreateCell()
    {
        IsAlive = true;

        //recreate biome
        ActiveRemoveOnDead(true);

        //if was builded at start, rebuild turret 
        BuildAtStart();
    }

    void ActiveRemoveOnDead(bool active)
    {
        //enable or disable every renderer
        Renderer[] renderers = toRemoveOnDead.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
            r.enabled = active;
    }

    #endregion

    #region public API

    /// <summary>
    /// Show turret without activate it
    /// </summary>
    public void ShowPreview()
    {
        //show preview only if is alive
        if (IsAlive == false)
        {
            //but if can recreate cell, show cost
            if(GameManager.instance.levelManager.levelConfig.CanRecreateCell)
                GameManager.instance.uiManager.SetCostText(true, true, resourcesToRecreateCell);

            return;
        }

        //do only if there is a turret to create
        if (turretToCreate == null)
            return;


        //do only if there isn't already a turret builded on it
        if ((turret != null && turret.IsPreview == false))
        {
            //but if can remove turret, show resources on sell
            if(canRemoveTurret)
                GameManager.instance.uiManager.SetCostText(true, false, resourcesOnSellTurret);

            return;
        }

        //else show preview

        //instantiate (with parent) or active it
        if (turret == null)
            turret = Instantiate(turretToCreate, transform);
        else
            turret.gameObject.SetActive(true);

        turret.ShowPreview();

        //show cost to create turret
        GameManager.instance.uiManager.SetCostText(true, true, resourcesToCreateTurret);
    }

    /// <summary>
    /// Remove preview
    /// </summary>
    public void HidePreview()
    {
        //check if there is a turret and is only a preview
        if (turret != null && turret.IsPreview)
            turret.gameObject.SetActive(false);

        //hide cost
        GameManager.instance.uiManager.SetCostText(false);
    }

    /// <summary>
    /// Player interact with the cell
    /// </summary>
    public bool Interact()
    {
        //if dead, try recreate cell
        if(IsAlive == false)
        {
            if (GameManager.instance.levelManager.levelConfig.CanRecreateCell && GameManager.instance.player.CurrentResources >= resourcesToRecreateCell)
            {
                RecreateCell();
                GameManager.instance.player.CurrentResources -= resourcesToRecreateCell;    //remove resources to recreate cell
                return true;
            }

            return false;
        }

        //else check if there is a turret to create
        if (turretToCreate == null)
            return false;

        //if there is already a turret, try remove it
        if (turret != null && turret.IsPreview == false)
        {
            if (canRemoveTurret)
            {
                RemoveBuildOnCell();
                GameManager.instance.player.CurrentResources += resourcesOnSellTurret;      //give resources from sell turret
                return true;
            }

            return false;
        }

        //else build
        if (GameManager.instance.player.CurrentResources >= resourcesToCreateTurret)
        {
            BuildOnCell();
            GameManager.instance.player.CurrentResources -= resourcesToCreateTurret;        //remove resources to create turret
            return true;
        }

        return false;
    }

    /// <summary>
    /// Kill the cell or lose the game
    /// </summary>
    public void KillCell(bool canEndGame = true)
    {
        //set got damage in this level
        GameManager.instance.levelManager.GetDamage();

        //do nothing if invincible
        if (isInvincible)
            return;

        //destroy cell or lose game (lose game only if canEndGame is true)
        if (IsAlive)
        {
            DestroyCell();

            //if only one life, call function again to kill definitively
            if (onlyOneLife)
                KillCell(canEndGame);
        }
        else if(canEndGame)
        {
            GameManager.instance.levelManager.EndGame(false);
        }
    }

    /// <summary>
    /// To call when load world and this cell was destroyed
    /// </summary>
    public void LoadDestroyedCell()
    {
        //destroy cell, but set is loaded
        DestroyCell(true);
    }

    public void ShowEnemyDestination(Enemy nearestEnemy)
    {
        //show enemy destination
        onShowEnemyDestination?.Invoke(nearestEnemy);
    }

    public void HideEnemyDestination()
    {
        //hide enemy destination
        onHideEnemyDestination?.Invoke();
    }

    #endregion
}
