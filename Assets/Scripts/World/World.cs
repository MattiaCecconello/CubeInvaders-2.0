using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using redd096;

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

    public Vector3 position => GameManager.instance.world.CoordinatesToPosition(this, 0);
    public Quaternion rotation => GameManager.instance.world.CoordinatesToRotation(this);

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

    public override string ToString()
    {
        return $"Face {face} ({x},{y})";
    }

    public static Coordinates operator +(Coordinates a, Vector2Int b) => new Coordinates(a.face, a.x + b.x, a.y + b.y);

    public static bool operator !=(Coordinates a, Coordinates b) => a.face != b.face || a.x != b.x || a.y != b.y;
    public static bool operator ==(Coordinates a, Coordinates b) => a.face == b.face && a.x == b.x && a.y == b.y;

    public override bool Equals(object obj)
    {
        return obj is Coordinates coordinates &&
               face == coordinates.face &&
               x == coordinates.x &&
               y == coordinates.y;
    }

    public override int GetHashCode()
    {
        int hashCode = 628288303;
        hashCode = hashCode * -1521134295 + face.GetHashCode();
        hashCode = hashCode * -1521134295 + x.GetHashCode();
        hashCode = hashCode * -1521134295 + y.GetHashCode();
        return hashCode;
    }
}

#endregion

#region save class

[System.Serializable]
public class WorldSave
{
    public List<Coordinates> coordinates = new List<Coordinates>();
    public List<bool> isAlive = new List<bool>();

    public WorldSave(List<Cell> cells)
    {
        //foreach cell, save start coordinates and values
        foreach (Cell cell in cells)
        {
            coordinates.Add(cell.startCoordinates);
            isAlive.Add(cell.IsAlive);
        }
    }
}

#endregion

[AddComponentMenu("Cube Invaders/World/World")]
public class World : MonoBehaviour
{
    #region variables

    [Header("Base")]
    public WorldConfig worldConfig;
    public RandomWorldConfig randomWorldConfig;

    [Header("Important")]
    public BiomesConfig biomesConfig;
    [SerializeField] bool resetCube = true;

    public System.Action onEndRotation;

    public Dictionary<Coordinates, Cell> Cells;

    WorldRotator worldRotator;

    #endregion

    void Awake()
    {
        GenerateReferences();

        //if not reset cube, try load
        if (resetCube == false)
            LoadCube();
    }

    void OnDestroy()
    {
        //be sure to reset event
        onEndRotation = null;
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

                //save start coordinates
                cell.startCoordinates = cell.coordinates;
            }
        }
    }

    void LoadCube()
    {
        //load class
        WorldSave load = SaveLoadJSON.Load<WorldSave>(worldConfig.name);

        if (load != null)
        {
            //foreach cell
            for(int i = 0; i < load.coordinates.Count; i++)
            {
                //if was destroyed, load a destroyed cell
                if (load.isAlive[i] == false)
                    Cells[load.coordinates[i]].LoadDestroyedCell();
            }
        }
    }

    #endregion

    #region regen world

    void RemoveOldWorld()
    {
        //remove every child
        foreach (Transform ch in transform)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.delayCall += () => DestroyImmediate(ch.gameObject);
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
#if UNITY_EDITOR
        Transform sun = (UnityEditor.PrefabUtility.InstantiatePrefab(worldConfig.SunPrefab) as GameObject).transform;
#else
        Transform sun = Instantiate(worldConfig.SunPrefab).transform;
#endif
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
                CreateAndSetCell(new Coordinates(EFace.front, x, y));

                //back
                CreateAndSetCell(new Coordinates(EFace.back, x, y));
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
                CreateAndSetCell(new Coordinates(EFace.right, z, y));

                //left
                CreateAndSetCell(new Coordinates(EFace.left, z, y));
            }
        }
    }

    void CreateUpDown()
    {
        for (int x = 0; x < worldConfig.NumberCells; x++)
        {
            //rotate 180 y axis just to look same direction (when rotate on local z axis - RotateAngleOrSide)
            for (int z = 0; z < worldConfig.NumberCells; z++)
            {
                //up
                CreateAndSetCell(new Coordinates(EFace.up, x, z));

                //down
                CreateAndSetCell(new Coordinates(EFace.down, x, z));
            }
        }
    }

    #endregion

    #region create cell

    void CreateAndSetCell(Coordinates coordinates)
    {
        //create cell
        Cell cell = CreateCell(coordinates);

        //set it
        Cells.Add(coordinates, cell);
        cell.coordinates = coordinates;
    }

    Cell CreateCell(Coordinates coordinates)
    {
        //create and set position and rotation
        Cell cell = InstantiateCellBasedOnFace(coordinates.face);
        cell.transform.position = CoordinatesToPosition(coordinates, 0);
        cell.transform.rotation = CoordinatesToRotation(coordinates);

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
#if UNITY_EDITOR
            case EFace.front:
                return UnityEditor.PrefabUtility.InstantiatePrefab(biomesConfig.Front) as Cell;
            case EFace.right:
                return UnityEditor.PrefabUtility.InstantiatePrefab(biomesConfig.Right) as Cell;
            case EFace.back:
                return UnityEditor.PrefabUtility.InstantiatePrefab(biomesConfig.Back) as Cell;
            case EFace.left:
                return UnityEditor.PrefabUtility.InstantiatePrefab(biomesConfig.Left) as Cell;
            case EFace.up:
                return UnityEditor.PrefabUtility.InstantiatePrefab(biomesConfig.Up) as Cell;
            case EFace.down:
                return UnityEditor.PrefabUtility.InstantiatePrefab(biomesConfig.Down) as Cell;
#else
            case EFace.front:
                return Instantiate(biomesConfig.Front);
            case EFace.right:
                return Instantiate(biomesConfig.Right);
            case EFace.back:
                return Instantiate(biomesConfig.Back);
            case EFace.left:
                return Instantiate(biomesConfig.Left);
            case EFace.up:
                return Instantiate(biomesConfig.Up);
            case EFace.down:
                return Instantiate(biomesConfig.Down);
#endif
        }

        return null;
    }

    #endregion

    #endregion

    #endregion

    #endregion

    #region public API

    /// <summary>
    /// Generate the world
    /// </summary>
    public void RegenWorld()
    {
        //remove old world
        RemoveOldWorld();

        //then create new world
        CreateWorld();
    }

    /// <summary>
    /// Rotate the cube
    /// </summary>
    /// <param name="coordinates">coordinates to rotate</param>
    /// <param name="lookingFace">rotation of the camera</param>
    /// <param name="rotateDirection">row (right, left) or column (up, down)</param>
    public void PlayerRotate(Coordinates coordinates, EFace lookingFace, ERotateDirection rotateDirection, float rotationTime)
    {
        worldRotator.PlayerRotate(new Coordinates[1] { coordinates }, lookingFace, rotateDirection, rotationTime);
    }

    /// <summary>
    /// Rotate the cube
    /// </summary>
    /// <param name="coordinates">coordinates to rotate</param>
    /// <param name="lookingFace">rotation of the camera</param>
    /// <param name="rotateDirection">row (right, left) or column (up, down)</param>
    public void PlayerRotate(Coordinates[] coordinates, EFace lookingFace, ERotateDirection rotateDirection, float rotationTime)
    {
        worldRotator.PlayerRotate(coordinates, lookingFace, rotateDirection, rotationTime);
    }

    /// <summary>
    /// Start random rotation
    /// </summary>
    public void RandomRotate()
    {
        new WorldRandomRotator(this).StartRandomize();
    }

    /// <summary>
    /// Enemy rotate cube
    /// </summary>
    public void RotateByEnemy(int numberRotations, float rotationTime)
    {
        //for n times, rotate row or column
        for (int i = 0; i < numberRotations; i++)
        {
            //randomize rotation
            EFace face = (EFace)Random.Range(0, 6);
            int x = Random.Range(0, worldConfig.NumberCells);
            int y = Random.Range(0, worldConfig.NumberCells);
            ERotateDirection randomDirection = (ERotateDirection)Random.Range(0, 4);

            //effective rotation
            worldRotator.Rotate(new Coordinates(face, x, y), EFace.front, randomDirection, rotationTime);
        }
    }

    /// <summary>
    /// Returns the position in the world of the cell at these coordinates
    /// <param name="distanceFromWorld">distance from the cell position</param>
    /// </summary>
    public Vector3 CoordinatesToPosition(Coordinates coordinates, float distanceFromWorld)
    {
        //position is index * size (then one axis is -distanceFromWorld or FaceSize + distanceFromWorld)
        Vector3 v = Vector3.zero;

        switch (coordinates.face)
        {
            case EFace.front:
                v.x = coordinates.x * worldConfig.CellsSize;
                v.y = coordinates.y * worldConfig.CellsSize;
                v.z = -distanceFromWorld;
                break;
            case EFace.right:
                v.x = worldConfig.FaceSize + distanceFromWorld;
                v.y = coordinates.y * worldConfig.CellsSize;
                v.z = coordinates.x * worldConfig.CellsSize;
                break;
            case EFace.back:
                //x inverse of front
                v.x = (worldConfig.NumberCells - 1 - coordinates.x) * worldConfig.CellsSize;
                v.y = coordinates.y * worldConfig.CellsSize;
                v.z = worldConfig.FaceSize + distanceFromWorld;
                break;
            case EFace.left:
                //z inverse of right
                v.x = -distanceFromWorld;
                v.y = coordinates.y * worldConfig.CellsSize;
                v.z = (worldConfig.NumberCells - 1 - coordinates.x) * worldConfig.CellsSize;
                break;
            case EFace.up:
                v.x = coordinates.x * worldConfig.CellsSize;
                v.y = worldConfig.FaceSize + distanceFromWorld;
                v.z = coordinates.y * worldConfig.CellsSize;
                break;
            case EFace.down:
                //z inverse of up
                v.x = coordinates.x * worldConfig.CellsSize;
                v.y = -distanceFromWorld;
                v.z = (worldConfig.NumberCells - 1 - coordinates.y) * worldConfig.CellsSize;
                break;
        }

        //is the angle in the lower left (front face) of the cube
        Vector3 cubeStartPosition = transform.position - worldConfig.HalfCube;

        //return start position + cell position + pivot position (cause we start from the angle of the cube, but we need the center of the cell as pivot)
        return cubeStartPosition + v + worldConfig.PivotBasedOnFace(coordinates.face);
    }

    public Quaternion CoordinatesToRotation(Coordinates coordinates)
    {
        //cell rotation based on face
        switch (coordinates.face)
        {
            case EFace.front:
                return Quaternion.Euler(0, 180, 0);
            case EFace.right:
                return Quaternion.Euler(0, 90, 0);
            case EFace.back:
                return Quaternion.Euler(0, 0, 0);
            case EFace.left:
                return Quaternion.Euler(0, -90, 0);
            case EFace.up:
                return Quaternion.Euler(-90, 180, 0);
            case EFace.down:
                return Quaternion.Euler(90, 180, 0);
            default:
                return Quaternion.identity;
        }
    }

    /// <summary>
    /// Get cells around (up, down, left, right) these coordinates
    /// </summary>
    public List<Cell> GetCellsAround(Coordinates coordinates)
    {
        List<Cell> cellsAround = new List<Cell>();
        Vector2Int[] directions = new Vector2Int[4] { Vector2Int.up, Vector2Int.down, Vector2Int.right, Vector2Int.left };

        //foreach direction
        foreach (Vector2Int direction in directions)
        {
            //if there is a cell and is != null
            if (Cells.ContainsKey(coordinates + direction))
            {
                Cell cell = Cells[coordinates + direction];
                if (cell != null)
                {
                    //add to the list
                    cellsAround.Add(cell);
                }
            }
        }

        return cellsAround;
    }

    /// <summary>
    /// Get every cell in this face
    /// </summary>
    public List<Cell> GetEveryCellInFace(EFace face)
    {
        //get cells in new face
        List<Cell> possibleCells = new List<Cell>();
        foreach (Coordinates coordinates in Cells.Keys)
        {
            if (coordinates.face == face)
                possibleCells.Add(GameManager.instance.world.Cells[coordinates]);
        }

        return possibleCells;
    }

    /// <summary>
    /// Return position and rotation at new coordinates
    /// </summary>
    public void GetPositionAndRotation(Coordinates coordinatesToAttack, float distance, out Vector3 position, out Quaternion rotation)
    {
        //coordinate position + distance from world
        position = CoordinatesToPosition(coordinatesToAttack, distance);

        //find direction to attack
        Vector3 direction = (coordinatesToAttack.position - position).normalized;

        //look in direction
        rotation = Quaternion.LookRotation(direction);
    }

    #endregion
}
