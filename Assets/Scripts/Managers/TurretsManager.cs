using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using redd096;

[AddComponentMenu("Cube Invaders/Manager/Turrets Manager")]
public class TurretsManager : MonoBehaviour
{
    Dictionary<EFace, List<BuildableObject>> buildableObjectsOnFace = new Dictionary<EFace, List<BuildableObject>>();

    public Dictionary<EFace, List<BuildableObject>> BuildableObjectsOnFace => buildableObjectsOnFace;

    void Update()
    {
        //update radar functions
        UpdateRadarEnemies();
    }

    #region public API

    public void AddTurretToDictionary(BuildableObject buildableObject)
    {
        //add key if dictionary no contains
        if (buildableObjectsOnFace.ContainsKey(buildableObject.CellOwner.coordinates.face) == false)
            buildableObjectsOnFace.Add(buildableObject.CellOwner.coordinates.face, new List<BuildableObject>());

        //add buildable object to the list
        buildableObjectsOnFace[buildableObject.CellOwner.coordinates.face].Add(buildableObject);
    }

    public void RemoveTurretFromDictionary(BuildableObject buildableObject)
    {
        //if dictionary has key && buildable object is in the list, remove it
        if (buildableObjectsOnFace.ContainsKey(buildableObject.CellOwner.coordinates.face) && buildableObjectsOnFace[buildableObject.CellOwner.coordinates.face].Contains(buildableObject))
            buildableObjectsOnFace[buildableObject.CellOwner.coordinates.face].Remove(buildableObject);
    }

    public List<BuildableObject> TurretsOnFace(EFace face)
    {
        //return list - if no key in the dictionary, return clear list
        if (buildableObjectsOnFace.ContainsKey(face))
            return new List<BuildableObject>(buildableObjectsOnFace[face]);

        return new List<BuildableObject>();
    }

    #endregion

    #region radar

    List<Cell> previousCellsInRadar = new List<Cell>();
    List<Cell> cellsInRadar = new List<Cell>();
    Dictionary<Coordinates, List<Enemy>> enemiesCoordinates = new Dictionary<Coordinates, List<Enemy>>();

    void UpdateRadarEnemies()
    {
        //OGNI VOLTA CHE SI ATTIVA UN RADAR, TUTTI I NEMICI SU QUELLA FACCIA MOSTRANO DESTINAZIONE E VITA (viene chiamato anche quando finisce di ruotare, se si riesce a riattivare)
        //OGNI VOLTA CHE SI DISATTIVA UN RADAR, SE NON CI SONO ALTRI RADAR SU QUELLA FACCIA, TUTTI I NEMICI SU QUELLA FACCIA NASCONDONO DESTINAZIONE E VITA (viene chiamato anche quando ruota)

        //NB di default cella e nemico nascondono vita e destination, quindi se viene chiamato Show() per primo, fallisce (questo dovrebbe già essere a posto per celle, ma bisogna assicurarsi l'health dei nemici)
        //NB che il destination sulle celle va aggiornato in base a quale nemico sia più vicino - quindi comunque nell'update avrà sia il resize dell'object che il find nearest enemy (ok per il resize, manca il find enemy)

        //NB che pure se vengono istanziati nuovi nemici dovranno mostrare la slider se c'è un radar presente, anche se non si è attivato o disattivato niente
        //NB bisogna tener presente che si possono ruotare celle senza radar e dovranno disabilitare l'enemyDestination e si può teletrasportare un enemy che dovrà disattivare la vita se non ha radar sulla nuova faccia
        //QUINDI servirebbe una chiamata ogni volta che un Enemy si sposta di faccia o viene istanziato (aggiunto alla dictionary del Wave Manager) - una chiamata anche ogni volta che muore un enemy
        //QUINDI servirebbe una chiamata ogni volta che viene fatta una rotazione (onWorldRotate di ogni Cell)

        //clear lists
        cellsInRadar.Clear();
        enemiesCoordinates.Clear();

        //foreach face
        foreach (EFace face in System.Enum.GetValues(typeof(EFace)))
        {
            //check there is a radar on this face && is active
            bool containsRadar = false;
            foreach (BuildableObject buildableObject in TurretsOnFace(face))
            {
                if (buildableObject && buildableObject is Radar && buildableObject.IsActive)
                {
                    containsRadar = true;
                    break;
                }
            }

            //enemy call if inside or outside radar area
            foreach (Enemy enemy in GameManager.instance.waveManager.EnemiesOnFace(face))
            {
                if (enemy == null)
                    continue;

                if (containsRadar)
                {
                    enemy.ShowHealth();

                    //add key if not in dictionary
                    if (enemiesCoordinates.ContainsKey(enemy.CoordinatesToAttack) == false)
                        enemiesCoordinates.Add(enemy.CoordinatesToAttack, new List<Enemy>());

                    enemiesCoordinates[enemy.CoordinatesToAttack].Add(enemy);   //add to dictionary based on coordinates
                }
                else
                {
                    enemy.HideHealth();
                }
            }
        }

        //foreach coordinates, find nearest enemy and show destination on cell
        foreach (Coordinates coordinates in enemiesCoordinates.Keys)
        {
            Enemy nearestToThisCell = enemiesCoordinates[coordinates].FindNearest(coordinates.position);

            //do only if enemy is near than minimum distance
            if (nearestToThisCell.DistanceFromCube > GameManager.instance.levelManager.generalConfig.minDistanceToShowDestination)
                continue;

            Cell cell = GameManager.instance.world.Cells[coordinates];
            cell.ShowEnemyDestination(nearestToThisCell);

            //add to list to check when hide destination
            cellsInRadar.Add(cell);
        }

        //foreach cell in previous list
        foreach (Cell previousCell in previousCellsInRadar)
        {
            //if not still in radar area
            if (cellsInRadar.Contains(previousCell) == false)
            {
                //hide destination
                previousCell.HideEnemyDestination();
            }
        }

        //set previous
        previousCellsInRadar = new List<Cell>(cellsInRadar);
    }

    #endregion

    #region no turrets on same face

    struct KeyNoTurretsOnSameFace
    {
        public EFace face;
        public BuildableObject turret;

        public KeyNoTurretsOnSameFace(EFace face, BuildableObject turret)
        {
            this.face = face;
            this.turret = turret;
        }
    }
    Dictionary<KeyNoTurretsOnSameFace, Coroutine> timerTurretsCoroutine = new Dictionary<KeyNoTurretsOnSameFace, Coroutine>();
    Dictionary<KeyNoTurretsOnSameFace, LineRenderer> feedbacksNoTurretsOnSameFace = new Dictionary<KeyNoTurretsOnSameFace, LineRenderer>();

    List<Turret> GetOnlyTurretsOnFace(KeyNoTurretsOnSameFace key, Turret turret, bool keepTurretCalled = true)
    {
        //get a list of only Turrets on this face
        List<Turret> turretsOnFace = new List<Turret>();
        foreach (BuildableObject buildableObject in TurretsOnFace(key.face))
        {
            if (buildableObject is Turret)
            {
                //not the turret called at least parameter is true
                if (keepTurretCalled || buildableObject != turret)
                {
                    if (key.turret == null                                              //be sure can be every turret
                        || buildableObject.CellOwner.TurretToCreate == key.turret)      //or are same type
                    {
                        turretsOnFace.Add(buildableObject as Turret);
                    }
                }
            }
        }

        return turretsOnFace;
    }

    public void TryStartTimer(Turret turret, EFace face)
    {
        KeyNoTurretsOnSameFace key = GameManager.instance.levelManager.levelConfig.OnlyIfSameType ?
            new KeyNoTurretsOnSameFace(face, turret.CellOwner.TurretToCreate) :     //key for this face and this type of turret
            new KeyNoTurretsOnSameFace(face, null);                                 //key with this face and no difference of turret

        //get a list of only Turrets on this face
        //if exceed limits on this level, start timer for this face
        if (GetOnlyTurretsOnFace(key, turret, true).Count > GameManager.instance.levelManager.levelConfig.LimitOfTurretsOnSameFace)
        {
            //stop coroutine already running, before restart
            if(timerTurretsCoroutine.ContainsKey(key))
            {
                StopCoroutine(timerTurretsCoroutine[key]);
                timerTurretsCoroutine.Remove(key);
            }

            timerTurretsCoroutine.Add(key, StartCoroutine(TimerBeforeDestroy_Coroutine(key)));
        }
    }

    public void TryStopTimer(Turret turret, EFace face)
    {
        KeyNoTurretsOnSameFace key = GameManager.instance.levelManager.levelConfig.OnlyIfSameType ?
            new KeyNoTurretsOnSameFace(face, turret.CellOwner.TurretToCreate) :     //key for this face and this type of turret
            new KeyNoTurretsOnSameFace(face, null);                                 //key with this face and no difference of turret

        //only if a coroutine is running
        if (timerTurretsCoroutine.ContainsKey(key) == false)
            return;

        //get a list of only Turrets on this face (but not this one calling the function, because is removed or is rotating)
        //if NOT exceed limits, stop timer on this face
        if (GetOnlyTurretsOnFace(key, turret, false).Count <= GameManager.instance.levelManager.levelConfig.LimitOfTurretsOnSameFace)
        {
            //stop coroutine already running
            if (timerTurretsCoroutine.ContainsKey(key))
            {
                StopCoroutine(timerTurretsCoroutine[key]);
                timerTurretsCoroutine.Remove(key);

                //be sure to remove feedback
                RemoveFeedbackTurretsOnSameFace(key);
            }
        }
    }

    IEnumerator TimerBeforeDestroy_Coroutine(KeyNoTurretsOnSameFace key)
    {
        float timer = 0;

        //update timer
        while (timer < 1)
        {
            timer += Time.deltaTime / GameManager.instance.levelManager.levelConfig.TimeBeforeDestroyTurretsOnSameFace;

            //update feedback
            UpdateFeedbackTurretsOnSameFace(key, timer);

            yield return null;
        }

        //when timer ends, destroy turrets on this face
        foreach (Turret turret in GetOnlyTurretsOnFace(key, null))
        {
            if (turret)
            {
                turret.RemoveTurret();
            }
        }

        //be sure to remove feedback
        RemoveFeedbackTurretsOnSameFace(key);
    }

    void UpdateFeedbackTurretsOnSameFace(KeyNoTurretsOnSameFace key, float timer)
    {
        //be sure there is a prefab to instantiate
        if (GameManager.instance.levelManager.levelConfig.line == null)
            return;

        //if there is no line, create one
        if (feedbacksNoTurretsOnSameFace.ContainsKey(key) == false)
        {
            feedbacksNoTurretsOnSameFace.Add(key, Instantiate(GameManager.instance.levelManager.levelConfig.line));
        }

        //be sure to activate
        feedbacksNoTurretsOnSameFace[key].gameObject.SetActive(true);

        //set color and positions
        SetColor(feedbacksNoTurretsOnSameFace[key], timer);
        SetPositions(key);
    }

    void SetColor(LineRenderer line, float delta)
    {
        //change line renderer color
        line.material.color = Color.Lerp(
            GameManager.instance.levelManager.levelConfig.line.sharedMaterial.color,    //from prefab
            GameManager.instance.levelManager.levelConfig.lineColorWhenExplode,         //to level config
            delta);                                                                     //based on delta
    }

    void SetPositions(KeyNoTurretsOnSameFace key)
    {
        //get turrets on face
        List<Turret> turretsOnFace = GetOnlyTurretsOnFace(key, null);

        //set positions
        feedbacksNoTurretsOnSameFace[key].positionCount = turretsOnFace.Count;
        for(int i = 0; i < turretsOnFace.Count; i++)
        {
            if (turretsOnFace[i])
            {
                Transform linePosition = turretsOnFace[i].GetComponent<TurretGraphics>().LinePosition;
                feedbacksNoTurretsOnSameFace[key].SetPosition(i, linePosition != null ? linePosition.position : turretsOnFace[i].transform.position);   //use line position or center of the turret
            }
        }
    }

    void RemoveFeedbackTurretsOnSameFace(KeyNoTurretsOnSameFace key)
    {
        //only if there is a line
        if (feedbacksNoTurretsOnSameFace.ContainsKey(key) == false)
            return;

        //disable it
        feedbacksNoTurretsOnSameFace[key].positionCount = 0;
        feedbacksNoTurretsOnSameFace[key].gameObject.SetActive(false);
    }

    #endregion
}
