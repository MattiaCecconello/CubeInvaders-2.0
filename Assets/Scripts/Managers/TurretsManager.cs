using System.Collections.Generic;
using UnityEngine;
using redd096;

[AddComponentMenu("Cube Invaders/Manager/Turrets Manager")]
public class TurretsManager : MonoBehaviour
{
    Dictionary<EFace, List<BuildableObject>> buildableObjectsOnFace = new Dictionary<EFace, List<BuildableObject>>();

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
            return buildableObjectsOnFace[face];

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
                if (buildableObject is Radar && buildableObject.IsActive)
                {
                    containsRadar = true;
                    break;
                }
            }

            //enemy call if inside or outside radar area
            foreach (Enemy enemy in GameManager.instance.waveManager.EnemiesOnFace(face))
            {
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
}
