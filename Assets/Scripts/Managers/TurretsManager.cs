using System.Collections.Generic;
using UnityEngine;

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

    void UpdateRadarEnemies()
    {
        //foreach face
        foreach(EFace face in System.Enum.GetValues(typeof(EFace)))
        {
            //check there is a radar on this face && is active
            bool containsRadar = false;
            foreach (BuildableObject buildableObject in buildableObjectsOnFace[face])
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
                    enemy.InRadarArea();
                else
                    enemy.OutRadarArea();
            }
        }
    }

    #endregion
}
