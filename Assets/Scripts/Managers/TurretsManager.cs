using System.Collections.Generic;
using UnityEngine;

public class TurretsManager : MonoBehaviour
{
    Dictionary<EFace, List<BuildableObject>> buildableObjectsOnFace = new Dictionary<EFace, List<BuildableObject>>();

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
}
