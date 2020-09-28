﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region enum & struct

[System.Serializable]
public enum ERotateDirection
{
    right, left, up, down
}

[System.Serializable]
public enum EFace
{
    front,
    right,
    back,
    left,
    up,
    down
}

[System.Serializable]
public struct Coordinates
{
    public EFace face;
    public int x;
    public int y;

    public Coordinates(EFace face, int x, int y)
    {
        this.face = face;
        this.x = x;
        this.y = y;
    }

    public Coordinates(EFace face, Vector2Int v)
    {
        this.face = face;
        this.x = v.x;
        this.y = v.y;
    }
}

#endregion

[AddComponentMenu("Cube Invaders/World/World")]
public class World : MonoBehaviour
{
    [Header("Regen")]
    [SerializeField] bool regen = false;

    [Header("Important")]
    public WorldConfig worldConfig = default;
    public BiomesConfig biomesConfig = default;

    public System.Action onEndRotation;

    public Dictionary<Coordinates, Cell> Cells;

    WorldRotator worldRotator;

    private void OnValidate()
    {
        //click regen to regenerate the world
        if(regen)
        {
            regen = false;

            //start regen
            RegenWorld();
        }
    }

    private void Awake()
    {
        GenerateReferences();
    }

    #region private API

    #region awake

    void GenerateReferences()
    {
        //create world rotator
        worldRotator = new WorldRotator(this);

        //create dictionary
        Cells = new Dictionary<Coordinates, Cell>();
        foreach (Transform child in transform)
        {
            Cell cell = child.GetComponent<Cell>();
            if (cell != null)
            {
                Cells.Add(cell.coordinates, cell);
            }
        }
    }

    #endregion

    #region regen world

    void RegenWorld()
    {
        //remove old world
        RemoveOldWorld();

        //then create new world
        CreateWorld();
    }

    void RemoveOldWorld()
    {
        //remove every child
        foreach (Transform ch in transform)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.delayCall += () =>
            {
                DestroyImmediate(ch.gameObject);
            };
#else
            Destroy(ch.gameObject);
#endif
        }

        //then clear dictionary
        Cells = new Dictionary<Coordinates, Cell>();
    }

    void CreateWorld()
    {
        //create sun
        InstantiateSun();

        //create front and back face
        CreateFrontBack();

        //create right and left face
        CreateRightLeft();

        //create up and down face
        CreateUpDown();
    }

    void InstantiateSun()
    {
        //create sun and set name, and position
        Transform sun = Instantiate(worldConfig.SunPrefab).transform;
        sun.name = "Sun";
        sun.position = transform.position;

        //set sun size
        float size = worldConfig.FaceSize - worldConfig.CellsSize;
        sun.localScale = new Vector3(size, size, size);

        //set parent
        sun.parent = transform;
    }

    #region cube

    #region create rows and columns

    void CreateFrontBack()
    {
        for (int x = 0; x < worldConfig.NumberCells; x++)
        {
            for (int y = 0; y < worldConfig.NumberCells; y++)
            {
                //front
                CreateAndSetCell(new Vector3(-90, 0, 0), new Coordinates(EFace.front, x, y));

                //back
                CreateAndSetCell(new Vector3(90, 0, 0), new Coordinates(EFace.back, x, y));
            }
        }
    }

    void CreateRightLeft()
    {
        for (int z = 0; z < worldConfig.NumberCells; z++)
        {
            for (int y = 0; y < worldConfig.NumberCells; y++)
            {
                //right
                CreateAndSetCell(new Vector3(0, 0, -90), new Coordinates(EFace.right, z, y));

                //left
                CreateAndSetCell(new Vector3(0, 0, 90), new Coordinates(EFace.left, z, y));
            }
        }
    }

    void CreateUpDown()
    {
        for (int x = 0; x < worldConfig.NumberCells; x++)
        {
            for (int z = 0; z < worldConfig.NumberCells; z++)
            {
                //up
                CreateAndSetCell(new Vector3(0, 0, 0), new Coordinates(EFace.up, x, z));

                //down
                CreateAndSetCell(new Vector3(180, 0, 0), new Coordinates(EFace.down, x, z));
            }
        }
    }

    #endregion

    #region create cell

    void CreateAndSetCell(Vector3 eulerRotation, Coordinates coordinates)
    {
        //create cell
        Cell cell = CreateCell(CoordinatesToPosition(coordinates), eulerRotation, coordinates.face);

        //set it
        Cells.Add(coordinates, cell);
        cell.coordinates = coordinates;
    }

    Cell CreateCell(Vector3 position, Vector3 eulerRotation, EFace face)
    {
        //create and set position and rotation
        Cell cell = InstantiateCellBasedOnFace(face);
        cell.transform.position = position;
        cell.transform.eulerAngles = eulerRotation;

        //set scale
        float size = worldConfig.CellsSize;
        cell.transform.localScale = new Vector3(size, size, size);

        //set parent
        cell.transform.parent = transform;

        return cell;
    }

    Cell InstantiateCellBasedOnFace(EFace face)
    {
        //create biome based on face
        switch (face)
        {
            case EFace.front:
                return Instantiate(biomesConfig.front);
            case EFace.right:
                return Instantiate(biomesConfig.right);
            case EFace.back:
                return Instantiate(biomesConfig.back);
            case EFace.left:
                return Instantiate(biomesConfig.left);
            case EFace.up:
                return Instantiate(biomesConfig.up);
            case EFace.down:
                return Instantiate(biomesConfig.down);
        }

        return null;
    }

    #endregion

    #endregion

    #endregion

    #endregion

    #region public API

    /// <summary>
    /// Rotate the cube
    /// </summary>
    /// <param name="startFace">face to start from</param>
    /// <param name="x">column</param>
    /// <param name="y">row</param>
    /// <param name="lookingFace">rotation of the camera</param>
    /// <param name="rotateDirection">row (right, left) or column (up, down)</param>
    public void Rotate(EFace startFace, int x, int y, EFace lookingFace, ERotateDirection rotateDirection)
    {
        worldRotator.Rotate(startFace, x, y, lookingFace, rotateDirection, worldConfig.RotationTime, true);
    }

    /// <summary>
    /// Rotate the cube
    /// </summary>
    /// <param name="startFace">face to start from</param>
    /// <param name="x">column</param>
    /// <param name="y">row</param>
    /// <param name="rotateDirection">row (right, left) or column (up, down)</param>
    /// <param name="rotationSpeed">time to do the animation</param>
    public void RandomRotate(EFace startFace, int x, int y, ERotateDirection rotateDirection, float rotationSpeed)
    {
        worldRotator.Rotate(startFace, x, y, EFace.front, rotateDirection, rotationSpeed, false);
    }

    /// <summary>
    /// Returns the position in the world of the cell at these coordinates
    /// </summary>
    public Vector3 CoordinatesToPosition(Coordinates coordinates)
    {
        //position is index * size (then one axis is 0 or FaceSize)
        Vector3 v = Vector3.zero;

        switch (coordinates.face)
        {
            case EFace.front:
                v.x = coordinates.x * worldConfig.CellsSize;
                v.y = coordinates.y * worldConfig.CellsSize;
                v.z = 0;
                break;
            case EFace.right:
                v.x = worldConfig.FaceSize;
                v.y = coordinates.y * worldConfig.CellsSize;
                v.z = coordinates.x * worldConfig.CellsSize;
                break;
            case EFace.back:
                //x inverse of front
                v.x = (worldConfig.NumberCells - 1 - coordinates.x) * worldConfig.CellsSize;
                v.y = coordinates.y * worldConfig.CellsSize;
                v.z = worldConfig.FaceSize;
                break;
            case EFace.left:
                //z inverse of right
                v.x = 0;
                v.y = coordinates.y * worldConfig.CellsSize;
                v.z = (worldConfig.NumberCells - 1 - coordinates.x) * worldConfig.CellsSize;
                break;
            case EFace.up:
                v.x = coordinates.x * worldConfig.CellsSize;
                v.y = worldConfig.FaceSize;
                v.z = coordinates.y * worldConfig.CellsSize;
                break;
            case EFace.down:
                //inverse of up
                v.x = coordinates.x * worldConfig.CellsSize;
                v.y = 0;
                v.z = (worldConfig.NumberCells - 1 - coordinates.y) * worldConfig.CellsSize;
                break;
        }

        //is the angle in the lower left (front face) of the cube
        Vector3 cubeStartPosition = transform.position - worldConfig.HalfCube;

        //return start position + cell position + pivot position (cause we start from the angle of the cube, but we need the center of the cell as pivot)
        return cubeStartPosition + v + worldConfig.PivotBasedOnFace(coordinates.face);
    }

    #endregion
}
