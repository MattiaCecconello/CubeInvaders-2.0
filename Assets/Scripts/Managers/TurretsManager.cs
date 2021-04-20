using System.Collections.Generic;
using UnityEngine;
using redd096;

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

        //NB di default cella e nemico nascondono vita e destination, quindi se viene chiamato Show() per primo, fallisce
        //NB che il destination sulle celle va aggiornato in base a quale nemico sia più vicino - quindi comunque nell'update avrà sia il resize dell'object che il find nearest enemy

        //foreach face
        foreach(EFace face in System.Enum.GetValues(typeof(EFace)))
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

            //clear lists
            cellsInRadar.Clear();
            enemiesCoordinates.Clear();

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

            //foreach coordinates, find nearest enemy and show destination on cell
            foreach(Coordinates coordinates in enemiesCoordinates.Keys)
            {
                Enemy nearestToThisCell = enemiesCoordinates[coordinates].FindNearest(coordinates.position);
                Cell cell = GameManager.instance.world.Cells[coordinates];
                cell.ShowEnemyDestination(nearestToThisCell);

                //add to list to check when hide destination
                cellsInRadar.Add(cell);
            }

            //foreach cell not in radar, hide destination if in previous list
            foreach (Cell cell in previousCellsInRadar)
                if (cellsInRadar.Contains(cell) == false)
                    cell.HideEnemyDestination();

            //set previous
            previousCellsInRadar = new List<Cell>(cellsInRadar);

        }
    }

    #endregion
}
