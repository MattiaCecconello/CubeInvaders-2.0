using UnityEngine;
using redd096;

#if UNITY_EDITOR

using UnityEditor;

[CustomEditor(typeof(Cell))]
public class CellEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(GUILayout.Button("Destroy Cell"))
        {
            //foreach cell selected, destroy it
            foreach(Object obj in targets)
            {
                Cell cell = obj as Cell;
                if (cell)
                    cell.DestroyCell();
            }
        }
        else if(GUILayout.Button("Recreate Cell"))
        {
            //foreach cell selected, recreate it
            foreach (Object obj in targets)
            {
                Cell cell = obj as Cell;
                if (cell)
                    cell.RecreateCell();
            }
        }
        else if(GUILayout.Button("Move To Current Coordinates"))
        {
            World world = FindObjectOfType<World>();

            //foreach cell selected, move to position based on coordinates
            foreach (Object obj in targets)
            {
                Cell cell = obj as Cell;
                if (cell)
                {
                    cell.transform.position = world.CoordinatesToPosition(cell.coordinates, 0);
                    cell.transform.rotation = world.CoordinatesToRotation(cell.coordinates);
                }
            }
        }
    }
}

#endif

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

    [Header("Descriptions")]
    [SerializeField] [TextArea] string descriptionToRecreateCell = "";
    [SerializeField] [TextArea] string descriptionToCreateTurret = "";
    [SerializeField] [TextArea] string descriptionToSellTurret = "";

    [Header("Debug")]
    public Coordinates coordinates;
    [ReadOnly] public Coordinates startCoordinates;
    [ReadOnly] [SerializeField] bool isAlive = true;

    //used from turret to know when is rotating
    public System.Action<Coordinates> onWorldRotate;
    public System.Action onDestroyCell;
    public System.Action<Enemy> onShowEnemyDestination;
    public System.Action onHideEnemyDestination;

    public BuildableObject turret { get; private set; }
    public bool IsAlive => isAlive;

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
            ShowPreview(false);
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

    //public only for editor call
    public void DestroyCell(bool loaded = false)
    {
        isAlive = false;

        //remove turret
        RemoveBuildOnCell();

        //remove biome
        ActiveRemoveOnDead(false);

        //call feedback only when loaded is false (only if playing, because this function is called by editor too)
        if (loaded == false && Application.isPlaying)
            onDestroyCell?.Invoke();
    }

    //public only for editor call
    public void RecreateCell()
    {
        isAlive = true;

        //recreate biome
        ActiveRemoveOnDead(true);

        //if was builded at start, rebuild turret (only if playing, because this function is called by editor too)
        if(Application.isPlaying)
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
    public void ShowPreview(bool buildedByPlayer = true)
    {
        //show preview only if is alive
        if (IsAlive == false)
        {
            //but if can recreate cell, show cost
            if (GameManager.instance.levelManager.levelConfig.CanRecreateCell)
            {
                GameManager.instance.uiManager.SetCostText(true, true, resourcesToRecreateCell);
                GameManager.instance.uiManager.SetTurretDescription(true, descriptionToRecreateCell);
            }

            return;
        }

        //do only if there is a turret to create
        if (turretToCreate == null)
            return;


        //do only if there isn't already a turret builded on it
        if ((turret != null && turret.IsPreview == false))
        {
            //but if can remove turret, show resources on sell
            if (canRemoveTurret)
            {
                GameManager.instance.uiManager.SetCostText(true, false, resourcesOnSellTurret);
                GameManager.instance.uiManager.SetTurretDescription(true, descriptionToSellTurret);
            }
            else if(GameManager.instance.levelManager.generalConfig.CanSellTurretsBuildedInThisWave && turret.isFirstWave)
            {
                GameManager.instance.uiManager.SetCostText(true, false, resourcesToCreateTurret);
                GameManager.instance.uiManager.SetTurretDescription(true, descriptionToSellTurret);
            }

            return;
        }

        //else show preview

        //instantiate (with parent) or active it
        if (turret == null)
            turret = Instantiate(turretToCreate, transform);
        else
            turret.gameObject.SetActive(true);

        turret.ShowPreview(buildedByPlayer);

        //show cost to create turret
        GameManager.instance.uiManager.SetCostText(true, true, resourcesToCreateTurret);
        GameManager.instance.uiManager.SetTurretDescription(true, descriptionToCreateTurret);
    }

    /// <summary>
    /// Remove preview
    /// </summary>
    public void HidePreview()
    {
        //check if there is a turret and is only a preview
        if (turret != null && turret.IsPreview)
            turret.gameObject.SetActive(false);

        //hide cost and description
        GameManager.instance.uiManager.SetCostText(false);
        GameManager.instance.uiManager.SetTurretDescription(false);
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
            //do only if can remove - or can remove turrets first wave, and this is the first wave for this turret
            if (canRemoveTurret || (GameManager.instance.levelManager.generalConfig.CanSellTurretsBuildedInThisWave && turret.isFirstWave))
            {
                RemoveBuildOnCell();
                GameManager.instance.player.CurrentResources += (canRemoveTurret ? resourcesOnSellTurret : resourcesToCreateTurret);      //give resources from sell turret (or regain used to create turret)
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
