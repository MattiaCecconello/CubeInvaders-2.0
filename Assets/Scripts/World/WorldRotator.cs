using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using redd096;

public struct RotationStruct
{
    public Coordinates[] coordinates;
    public EFace lookingFace;
    public ERotateDirection rotateDirection;
    public float rotationTime;
    public bool canSkipAnimation;

    public RotationStruct(Coordinates[] coordinates, EFace lookingFace, ERotateDirection rotateDirection, float rotationTime, bool canSkipAnimation)
    {
        this.coordinates = coordinates;
        this.lookingFace = lookingFace;
        this.rotateDirection = rotateDirection;
        this.rotationTime = rotationTime;
        this.canSkipAnimation = canSkipAnimation;
    }
}

public class WorldRotator
{
    #region variables

    protected World world;
    Transform rotatorParent;
    Transform RotatorParent { get
        { 
            //if null, create empty object
            if (rotatorParent == null)
            {
                rotatorParent = new GameObject("RotatorParent").transform;
            }

            //get rotator parent in transform position
            rotatorParent.position = world.transform.position;
            return rotatorParent;
        } }

    Coordinates[] coordinatesToRotate;
    List<Cell> cellsToRotate = new List<Cell>();
    List<Coordinates> cellsKeys = new List<Coordinates>();
    Coroutine rotatingWorld_Coroutine;

    Queue<RotationStruct> rotationsToDo = new Queue<RotationStruct>();
    bool skippingRotations = false;
    Queue<RotationStruct> rotationsWaitingSkip = new Queue<RotationStruct>();

    #endregion

    public WorldRotator(World world)
    {
        this.world = world;
    }

    void DoRotation()
    {
        RotationStruct rotation = rotationsToDo.Peek();

        //set variables
        coordinatesToRotate = rotation.coordinates;
        cellsToRotate.Clear();
        cellsKeys.Clear();

        //every coordinates can be only on the same face
        EFace startFace = coordinatesToRotate[0].face;

        //rotate row
        if (rotation.rotateDirection == ERotateDirection.right || rotation.rotateDirection == ERotateDirection.left)
        {
            bool forward = rotation.rotateDirection == ERotateDirection.right;

            if (startFace == EFace.up || startFace == EFace.down)
            {
                //if face up or face down, the inputs are differents based on the rotation of the camera
                switch (rotation.lookingFace)
                {
                    case EFace.front:
                        RotateUpDownRow(startFace, forward);
                        break;
                    case EFace.right:
                        if (startFace == EFace.up)
                            RotateFrontColumn(startFace, forward);
                        else
                            RotateFrontColumn(startFace, !forward);
                        break;
                    case EFace.back:
                        RotateUpDownRow(startFace, !forward);
                        break;
                    case EFace.left:
                        if (startFace == EFace.up)
                            RotateFrontColumn(startFace, !forward);
                        else
                            RotateFrontColumn(startFace, forward);
                        break;
                }
            }
            else
            {
                //else just rotate row lateral faces
                RotateLateralRow(forward);
            }
        }
        //rotate column
        else
        {
            bool forward = rotation.rotateDirection == ERotateDirection.up;

            //if face up or face down, the inputs are differents based on the rotation of the camera
            if (startFace == EFace.up || startFace == EFace.down)
            {
                switch (rotation.lookingFace)
                {
                    case EFace.front:
                        RotateFrontColumn(startFace, forward);
                        break;
                    case EFace.right:
                        if (startFace == EFace.up)
                            RotateUpDownRow(startFace, !forward);
                        else
                            RotateUpDownRow(startFace, forward);
                        break;
                    case EFace.back:
                        RotateFrontColumn(startFace, !forward);
                        break;
                    case EFace.left:
                        if (startFace == EFace.up)
                            RotateUpDownRow(startFace, forward);
                        else
                            RotateUpDownRow(startFace, !forward);
                        break;
                }
            }
            else
            {
                //else just rotate column
                if (startFace == EFace.right || startFace == EFace.left)
                {
                    //rotate column face right or left
                    RotateRightLeftColumn(startFace, forward);
                }
                else
                {
                    //rotate column front faces (front, up, back, down)
                    RotateFrontColumn(startFace, forward);
                }
            }
        }
    }

    #region private API

    #region general

    void OnWorldRotate()
    {
        //foreach cell to rotate
        foreach (Cell cell in cellsToRotate)
        {
            //if the cell isn't null, call onWorldRotate
            if (cell)
                cell.onWorldRotate?.Invoke(cell.coordinates);
        }
    }

    void SetCoordinates(Cell oldCell, Coordinates newCoords)
    {
        //set new cell in dictionary
        world.Cells[newCoords] = oldCell;

        //and update coordinates in game object
        world.Cells[newCoords].coordinates = newCoords;
    }

    Coordinates UpdateCoordinatesCompleteFace(Coordinates coordinates, bool forward)
    {
        if (forward)
        {
            //rotate the face, so change coordinates x and y, but not the face

            Vector2Int v = Vector2Math.InverseEqual(coordinates.x, coordinates.y, world.worldConfig.NumberCells);
            coordinates.x = v.x;
            coordinates.y = v.y;
        }
        else
        {
            //rotate the face, so change coordinates x and y, but not the face

            Vector2Int v = Vector2Math.EqualInverse(coordinates.x, coordinates.y, world.worldConfig.NumberCells);
            coordinates.x = v.x;
            coordinates.y = v.y;
        }

        return coordinates;
    }

    void SelectCell(Coordinates coordinates)
    {
        //only if not already in the lists
        if (!cellsKeys.Contains(coordinates))
        {
            //add cell and coordinates
            cellsToRotate.Add(world.Cells[coordinates]);
            cellsKeys.Add(coordinates);
        }
    }

    void SelectAllFace(int line, EFace face1, EFace face2)
    {
        //select all face 1 or all face 2
        if (line <= 0 || line >= world.worldConfig.NumberCells - 1)
        {
            if (world.worldConfig.NumberCells > 1)
            {
                EFace face = line <= 0 ? face1 : face2;
                SelectAllFaceCells(face);
            }
            else
            {
                //if only one cell, then select both faces
                SelectAllFaceCells(face1);
                SelectAllFaceCells(face2);
            }
        }
    }

    void SelectAllFaceCells(EFace face)
    {
        //add cell and coordinates for every row and column on this face
        for (int x = 0; x < world.worldConfig.NumberCells; x++)
        {
            for (int y = 0; y < world.worldConfig.NumberCells; y++)
            {
                SelectCell(new Coordinates(face, x, y));
            }
        }
    }

    #endregion

    #region animations

    protected virtual float GetAnimationCurveValue(float delta)
    {
        return world.worldConfig.RotationAnimationCurve.Evaluate(delta);
    }

    IEnumerator AnimationRotate(Vector3 rotateAxis, bool forward)
    {
        //call event on world rotate
        OnWorldRotate();

        //set parent
        cellsToRotate.SetParent(RotatorParent);

        //we need to do a 90° rotation
        float rotationToReach = forward ? 90 : -90;

        //animation
        float delta = 0;
        while (delta < 1)
        {
            delta += Time.deltaTime / rotationsToDo.Peek().rotationTime;

            float rotated = Mathf.Lerp(0, rotationToReach, GetAnimationCurveValue(delta));
            RotatorParent.eulerAngles = rotateAxis * rotated;

            //if skipping rotations, skip this one
            if (skippingRotations)
            {
                break;
            }

            yield return null;
        }

        //final rotation
        RotatorParent.eulerAngles = rotateAxis * rotationToReach;

        //remove parent and reset its rotation
        cellsToRotate.SetParent(world.transform);
        RotatorParent.rotation = Quaternion.identity;

        //remove from the list
        rotationsToDo.Dequeue();

        //call end rotation
        world.onEndRotation?.Invoke();
        rotatingWorld_Coroutine = null;

        //if skipping rotations and finished rotations
        if(skippingRotations && rotationsToDo.Count <= 0)
        {
            //finish skip
            skippingRotations = false;

            //then copy rotations waiting skip in rotations to do, and reset rotations waiting skip
            if(rotationsWaitingSkip.Count > 0)
            {
                rotationsToDo = new Queue<RotationStruct>(rotationsWaitingSkip);
                rotationsWaitingSkip.Clear();
            }
        }

        //if there are other rotations to do, start it
        if (rotationsToDo.Count > 0)
            DoRotation();
    }

    #endregion

    #region rotate row

    #region front, right, back, left

    void RotateLateralRow(bool toRight)
    {
        //foreach coordinate, use y to select
        foreach (Coordinates coordinates in coordinatesToRotate)
        {
            int y = coordinates.y;

            //rotate y
            SelectLateralRowCells(y);
        }

        //rotate animation
        if(world.gameObject.activeInHierarchy)
            rotatingWorld_Coroutine = world.StartCoroutine(AnimationRotate(Vector3.up, !toRight));

        //update dictionary
        UpdateDictionaryLateralRow(toRight);
    }

    void SelectLateralRowCells(int y)
    {
        //select y in every lateral face
        for (int faceIndex = 0; faceIndex < 4; faceIndex++)
        {
            for (int x = 0; x < world.worldConfig.NumberCells; x++)
            {
                SelectCell(new Coordinates((EFace)faceIndex, x, y));
            }
        }

        //select all face down or up
        SelectAllFace(y, EFace.down, EFace.up);
    }

    void UpdateDictionaryLateralRow(bool toRight)
    {
        Dictionary<Coordinates, Cell> oldCells = world.Cells.CreateCopy();

        foreach (Coordinates previousCoords in cellsKeys)
        {
            Coordinates newCoords = previousCoords;

            if (previousCoords.face != EFace.up && previousCoords.face != EFace.down)
            {
                //get coords same position but next or prev face
                newCoords.face = (EFace)WorldUtility.SelectIndex((int)previousCoords.face, toRight, 4);

                //face: front -> right   //right -> back   //back -> left   //left -> front
            }
            else
            {
                //rotate the row, so change coordinates x and y, but not the face
                bool rotateToRight = previousCoords.face == EFace.up ? toRight : !toRight;
                newCoords = UpdateCoordinatesCompleteFace(previousCoords, rotateToRight);
            }

            //set new coordinates
            SetCoordinates(oldCells[previousCoords], newCoords);
        }
    }

    #endregion

    #region up and down

    void RotateUpDownRow(EFace startFace, bool toRight)
    {
        //foreach coordinate, use y to select
        foreach (Coordinates coordinates in coordinatesToRotate)
        {
            int y = coordinates.y;

            //rotate y. Down face is the inverse
            if (startFace == EFace.up)
            {
                SelectUpDownRowCells(y);
            }
            else
            {
                SelectUpDownRowCells(WorldMath.InverseN(y, world.worldConfig.NumberCells));
            }
        }

        //in the down face is inverse
        if (startFace == EFace.down)
            toRight = !toRight;

        //rotate animation
        if(world.gameObject.activeInHierarchy)
            rotatingWorld_Coroutine = world.StartCoroutine(AnimationRotate(Vector3.forward, !toRight));

        //update dictionary
        UpdateDictionaryUpDownRow(toRight);
    }

    void SelectUpDownRowCells(int y)
    {
        //select y in up, right, down, left face
        for (int faceIndex = 0; faceIndex < 4; faceIndex++)
        {
            //set f equal to faceIndex, but instead of front and back, use up and down
            EFace face = (EFace)faceIndex;

            if ((EFace)faceIndex == EFace.front) face = EFace.up;
            if ((EFace)faceIndex == EFace.back) face = EFace.down;

            for (int x = 0; x < world.worldConfig.NumberCells; x++)
            {
                switch (face)
                {
                    case EFace.up:
                        //up select the row
                        SelectCell(new Coordinates(face, x, y));
                        break;
                    case EFace.down:
                        //down select the row but inverse of up
                        SelectCell(new Coordinates(face, x, WorldMath.InverseN(y, world.worldConfig.NumberCells)));
                        break;
                    case EFace.right:
                        //right select the column instead of row
                        SelectCell(new Coordinates(face, y, x));
                        break;
                    case EFace.left:
                        //left select the column instead of row
                        //but if you are rotating the first row, this is the last column, if you are rotating the last row, this is the first column
                        SelectCell(new Coordinates(face, WorldMath.InverseN(y, world.worldConfig.NumberCells), x));
                        break;
                }
            }
        }

        //select all face front or back
        SelectAllFace(y, EFace.front, EFace.back);
    }

    void UpdateDictionaryUpDownRow(bool toRight)
    {
        Dictionary<Coordinates, Cell> oldCells = world.Cells.CreateCopy();

        foreach (Coordinates previousCoords in cellsKeys)
        {
            Coordinates newCoords = previousCoords;
            
            if(previousCoords.face != EFace.front && previousCoords.face != EFace.back)
            {
                //change face and also coordinates
                newCoords = CoordsToRight(previousCoords, toRight);
            }
            else
            {
                //rotate the row, so change coordinates x and y, but not the face
                bool rotateToRight = previousCoords.face == EFace.back ? toRight : !toRight;
                newCoords = UpdateCoordinatesCompleteFace(previousCoords, rotateToRight);
            }

            //set new coordinates
            SetCoordinates(oldCells[previousCoords], newCoords);
        }
    }

    Coordinates CoordsToRight(Coordinates coordinates, bool toRight)
    {
        //calculate new face
        coordinates.face = WorldUtility.FindFaceUpToRight(coordinates.face, toRight);

        //get coordinates x,y of face to the right or to the left
        if (toRight)
        {
            Vector2Int v = Vector2Math.EqualInverse(coordinates.x, coordinates.y, world.worldConfig.NumberCells);
            coordinates.x = v.x;
            coordinates.y = v.y;
        }
        else
        {
            Vector2Int v = Vector2Math.InverseEqual(coordinates.x, coordinates.y, world.worldConfig.NumberCells);
            coordinates.x = v.x;
            coordinates.y = v.y;
        }

        return coordinates;
    }

    #endregion

    #endregion

    #region rotate column

    #region front, up, back, down

    void RotateFrontColumn(EFace startFace, bool toUp)
    {
        //foreach coordinate, use x to select
        foreach (Coordinates coordinates in coordinatesToRotate)
        {
            int x = coordinates.x;

            //rotate x. Back face is the inverse
            if (startFace != EFace.back)
            {
                SelectFrontColumnCells(x);
            }
            else
            {
                SelectFrontColumnCells(WorldMath.InverseN(x, world.worldConfig.NumberCells));
            }
        }

        //in the back face is inverse
        if (startFace == EFace.back)
            toUp = !toUp;

        //rotate animation
        if(world.gameObject.activeInHierarchy)
            rotatingWorld_Coroutine = world.StartCoroutine(AnimationRotate(Vector3.right, toUp));

        //update dictionary
        UpdateDictionaryFrontColumn(toUp);
    }

    void SelectFrontColumnCells(int x)
    {        
        //select x in every front face
        for (int faceIndex = 0; faceIndex < 4; faceIndex++)
        {
            //set f equal to faceIndex, but instead of right and left, use up and down
            EFace face = (EFace)faceIndex;

            if ((EFace)faceIndex == EFace.right) face = EFace.up;
            if ((EFace)faceIndex == EFace.left) face = EFace.down;

            for (int y = 0; y < world.worldConfig.NumberCells; y++)
            {
                //line is equal to x
                //but when is face back is the inverse of the other faces, so column 0 is 2, column 1 is 1, column 2 is 0
                int line = face != EFace.back ? x : WorldMath.InverseN(x, world.worldConfig.NumberCells);

                SelectCell(new Coordinates(face, line, y));
            }
        }

        //select all face right or left
        SelectAllFace(x, EFace.left, EFace.right);
    }

    void UpdateDictionaryFrontColumn(bool toUp)
    {
        Dictionary<Coordinates, Cell> oldCells = world.Cells.CreateCopy();

        foreach (Coordinates previousCoords in cellsKeys)
        {
            Coordinates newCoords = previousCoords;

            if (previousCoords.face != EFace.right && previousCoords.face != EFace.left)
            {
                //change face and coordinates
                newCoords = CoordsFrontColumn(previousCoords, toUp);
            }
            else
            {
                //rotate the column, so change coordinates x and y, but not the face
                bool rotateToUp = previousCoords.face == EFace.left ? toUp : !toUp;
                newCoords = UpdateCoordinatesCompleteFace(previousCoords, rotateToUp);
            }

            //set new coordinates
            SetCoordinates(oldCells[previousCoords], newCoords);
        }
    }

    Coordinates CoordsFrontColumn(Coordinates coordinates, bool toUp)
    {
        Coordinates newCoords = coordinates;

        //calculate new face
        newCoords.face = WorldUtility.FindFaceFrontToUp(coordinates.face, toUp);

        //get coordinates x,y of face to the top or to the down
        if (coordinates.face == EFace.back || newCoords.face == EFace.back)
        {
            //if the prev face or next face is Face.back, then you Self_InverseInverse
            Vector2Int v = Vector2Math.Self_InverseInverse(coordinates.x, coordinates.y, world.worldConfig.NumberCells);
            newCoords.x = v.x;
            newCoords.y = v.y;
        }

        return newCoords;
    }

    #endregion

    #region right and left

    void RotateRightLeftColumn(EFace startFace, bool toUp)
    {
        //foreach coordinate, use x to select
        foreach (Coordinates coordinates in coordinatesToRotate)
        {
            int x = coordinates.x;

            //right face. Left face is the inverse
            if (startFace == EFace.right)
            {
                SelectRightLeftColumnCells(x);
            }
            else
            {
                SelectRightLeftColumnCells(WorldMath.InverseN(x, world.worldConfig.NumberCells));
            }
        }

        //in the left is inverse
        if (startFace == EFace.left)
            toUp = !toUp;

        //rotate animation
        if(world.gameObject.activeInHierarchy)
            rotatingWorld_Coroutine = world.StartCoroutine(AnimationRotate(Vector3.forward, toUp));

        //update dictionary
        UpdateDictionaryRightLeftColumn(toUp);
    }

    void SelectRightLeftColumnCells(int x)
    {
        //select line in up, right, down, left face
        for (int faceIndex = 0; faceIndex < 4; faceIndex++)
        {
            //set f equal to faceIndex, but instead of front and back, use up and down
            EFace face = (EFace)faceIndex;

            if ((EFace)faceIndex == EFace.front) face = EFace.up;
            if ((EFace)faceIndex == EFace.back) face = EFace.down;

            for (int y = 0; y < world.worldConfig.NumberCells; y++)
            {
                switch (face)
                {
                    case EFace.right:
                        //select column
                        SelectCell(new Coordinates(face, x, y));
                        break;
                    case EFace.left:
                        //left select the column, but inverse (when select 0 is the last, when select last is the 0)
                        SelectCell(new Coordinates(face, WorldMath.InverseN(x, world.worldConfig.NumberCells), y));
                        break;
                    case EFace.up:
                        //up select the row instead of column
                        SelectCell(new Coordinates(face, y, x));
                        break;
                    case EFace.down:
                        //down is inverse of up
                        SelectCell(new Coordinates(face, y, WorldMath.InverseN(x, world.worldConfig.NumberCells)));
                        break;
                }
            }
        }

        //select all face front or back
        SelectAllFace(x, EFace.front, EFace.back);
    }

    void UpdateDictionaryRightLeftColumn(bool toUp)
    {
        Dictionary<Coordinates, Cell> oldCells = world.Cells.CreateCopy();

        foreach (Coordinates previousCoords in cellsKeys)
        {
            Coordinates newCoords = previousCoords;

            if (previousCoords.face != EFace.front && previousCoords.face != EFace.back)
            {
                //change face, and in back face also coordinates
                newCoords = CoordsRightLeftColumn(previousCoords, toUp);
            }
            else
            {
                //rotate the column, so change coordinates x and y, but not the face
                bool rotateToUp = previousCoords.face == EFace.front ? toUp : !toUp;
                newCoords = UpdateCoordinatesCompleteFace(previousCoords, rotateToUp);
            }

            //set new coordinates
            SetCoordinates(oldCells[previousCoords], newCoords);
        }
    }

    Coordinates CoordsRightLeftColumn(Coordinates coordinates, bool toUp)
    {
        //calculate new face -> work the inverse, so we use !toUp
        coordinates.face = WorldUtility.FindFaceUpToRight(coordinates.face, !toUp);

        //get coordinates x,y of face to the top or face to the bottom
        if (toUp)
        {
            Vector2Int v = Vector2Math.InverseEqual(coordinates.x, coordinates.y, world.worldConfig.NumberCells);
            coordinates.x = v.x;
            coordinates.y = v.y;
        }
        else
        {
            Vector2Int v = Vector2Math.EqualInverse(coordinates.x, coordinates.y, world.worldConfig.NumberCells);
            coordinates.x = v.x;
            coordinates.y = v.y;
        }

        return coordinates;
    }

    #endregion

    #endregion

    #endregion

    #region public API

    public void PlayerRotate(Coordinates[] coordinates, EFace lookingFace, ERotateDirection rotateDirection, float rotationTime)
    {
        //do nothing if is running a rotation not skippable (enemy rotation)
        if (rotatingWorld_Coroutine != null && rotationsToDo.Peek().canSkipAnimation == false)
            return;

        //else start rotation (can skip)
        Rotate(new RotationStruct(coordinates, lookingFace, rotateDirection, rotationTime, true));
    }

    public void Rotate(Coordinates coordinates, EFace lookingFace, ERotateDirection rotateDirection, float rotationTime)
    {
        //start rotation (can NOT skip)
        Rotate(new RotationStruct(new Coordinates[1] { coordinates }, lookingFace, rotateDirection, rotationTime, false));
    }

    public void Rotate(RotationStruct rotationToDo)
    {
        //if is running a skippable rotation, skip every skippable rotation
        if (rotatingWorld_Coroutine != null && rotationsToDo.Peek().canSkipAnimation)
        {
            skippingRotations = true;

            //add to list rotations waiting skip
            rotationsWaitingSkip.Enqueue(rotationToDo);
        }
        else
        {
            //add to list
            rotationsToDo.Enqueue(rotationToDo);

            //if there is no rotation running, start rotation
            if (rotatingWorld_Coroutine == null)
                DoRotation();
        }
    }

    #endregion
}
